namespace Dorian.Application.Nutrition;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Application.WorkoutActivities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class NutritionService : INutritionService
{
    private const string Disclaimer = "La información nutricional es referencial y no reemplaza una consulta profesional.";

    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWorkoutActivityService _workoutActivityService;

    public NutritionService(IDorianDbContext dbContext, ICurrentUserService currentUserService, IWorkoutActivityService workoutActivityService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _workoutActivityService = workoutActivityService;
    }

    public async Task<NutritionProfileResponse?> GetMyProfileAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var profile = await _dbContext.NutritionProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);
        return profile is null ? null : Map(profile);
    }

    public async Task<NutritionProfileResponse?> GetProfileByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForViewAsync(customerId, cancellationToken);
        var profile = await _dbContext.NutritionProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);
        return profile is null ? null : Map(profile);
    }

    public async Task<NutritionProfileResponse> GenerateMyProfileAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var fitnessProfile = await RequireFitnessProfileAsync(customer.Id, cancellationToken);
        var composition = await BuildCompositionAsync(customer.Id, fitnessProfile, cancellationToken);
        var activitySummary = await _workoutActivityService.GetMySummaryAsync(28, cancellationToken);
        var nutrition = BuildNutritionTargets(fitnessProfile, composition, activitySummary);

        var profile = await _dbContext.NutritionProfiles.SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);
        if (profile is null)
        {
            profile = new NutritionProfile(
                Guid.NewGuid(),
                customer.Id,
                fitnessProfile.Goal,
                nutrition.DailyCaloriesTarget,
                nutrition.ProteinGrams,
                nutrition.CarbsGrams,
                nutrition.FatGrams,
                nutrition.MealsPerDay,
                nutrition.WaterLitersTarget,
                null);
            _dbContext.NutritionProfiles.Add(profile);
        }
        else
        {
            profile.Update(
                fitnessProfile.Goal,
                nutrition.DailyCaloriesTarget,
                nutrition.ProteinGrams,
                nutrition.CarbsGrams,
                nutrition.FatGrams,
                nutrition.MealsPerDay,
                nutrition.WaterLitersTarget,
                profile.DietaryRestrictions);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    public async Task<NutritionProfileResponse> UpdateMyProfileAsync(UpdateNutritionProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.MealsPerDay < 3 || request.MealsPerDay > 6)
        {
            throw new NutritionValidationException("Las comidas por día deben estar entre 3 y 6.");
        }

        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var profile = await _dbContext.NutritionProfiles.SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NotFoundException("Nutrition profile not found.");

        profile.Update(
            profile.Goal,
            profile.DailyCaloriesTarget,
            profile.ProteinGrams,
            profile.CarbsGrams,
            profile.FatGrams,
            request.MealsPerDay,
            profile.WaterLitersTarget,
            request.DietaryRestrictions);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    public async Task<IReadOnlyCollection<MealPlanResponse>> GetMyMealPlanAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await LoadMealPlanAsync(customer.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MealPlanResponse>> GetMealPlanByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForViewAsync(customerId, cancellationToken);
        return await LoadMealPlanAsync(customer.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MealPlanResponse>> GenerateMyMealPlanAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var nutritionProfile = await _dbContext.NutritionProfiles.SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NutritionValidationException("Genera primero tu perfil nutricional.");
        var fitnessProfile = await RequireFitnessProfileAsync(customer.Id, cancellationToken);

        var existing = await _dbContext.MealPlans.Where(x => x.CustomerId == customer.Id).ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            _dbContext.MealPlans.RemoveRange(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var trainingDays = fitnessProfile.GetTrainingDays().ToArray();
        var days = trainingDays.Length > 0 ? trainingDays : [TrainingDay.Monday];
        var blueprints = BuildMealBlueprints(fitnessProfile.Goal, nutritionProfile);

        foreach (var day in days)
        {
            var mealPlan = new MealPlan(Guid.NewGuid(), customer.Id, $"Plan {day}", $"Distribución diaria sugerida para {day}.", day);
            foreach (var item in blueprints)
            {
                mealPlan.Items.Add(new MealItem(
                    Guid.NewGuid(),
                    mealPlan.Id,
                    item.MealType,
                    item.Name,
                    item.Description,
                    item.Calories,
                    item.ProteinGrams,
                    item.CarbsGrams,
                    item.FatGrams));
            }

            _dbContext.MealPlans.Add(mealPlan);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await LoadMealPlanAsync(customer.Id, cancellationToken);
    }

    private async Task<IReadOnlyCollection<MealPlanResponse>> LoadMealPlanAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.MealPlans
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);
    }

    private async Task<CustomerFitnessProfile> RequireFitnessProfileAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.CustomerFitnessProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken)
            ?? throw new NutritionValidationException("Completa tu onboarding fitness antes de generar nutrición.");
        if (!profile.OnboardingCompleted)
        {
            throw new NutritionValidationException("Completa tu onboarding fitness antes de generar nutrición.");
        }

        return profile;
    }

    private async Task<BodyCompositionSnapshot> BuildCompositionAsync(Guid customerId, CustomerFitnessProfile fitnessProfile, CancellationToken cancellationToken)
    {
        var latestMeasurement = await _dbContext.BodyMeasurements
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.MeasuredAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var weight = latestMeasurement?.WeightKg ?? fitnessProfile.WeightKg;
        var height = latestMeasurement?.HeightCm ?? fitnessProfile.HeightCm;
        var age = Math.Max(16, DateTime.UtcNow.Year - fitnessProfile.BirthDate.Year - (DateOnly.FromDateTime(DateTime.UtcNow.Date) < fitnessProfile.BirthDate.AddYears(DateTime.UtcNow.Year - fitnessProfile.BirthDate.Year) ? 1 : 0));
        return new BodyCompositionSnapshot(weight, height, age, fitnessProfile.Gender);
    }

    private static NutritionTargets BuildNutritionTargets(CustomerFitnessProfile fitnessProfile, BodyCompositionSnapshot composition, ActivitySummaryResponse activitySummary)
    {
        var baseCalories = (10m * composition.WeightKg) + (6.25m * composition.HeightCm) - (5m * composition.Age) + (composition.Gender == FitnessProfileGender.Female ? -161m : 5m);
        var activityFactor = ResolveActivityFactor(fitnessProfile.ExperienceLevel, activitySummary.DaysTrained);
        var maintenance = baseCalories * activityFactor;
        var targetCalories = fitnessProfile.Goal switch
        {
            FitnessGoal.LoseWeight => maintenance - 350m,
            FitnessGoal.Hypertrophy => maintenance + 300m,
            FitnessGoal.Definition => maintenance - 180m,
            _ => maintenance
        };

        var proteinPerKg = fitnessProfile.Goal switch
        {
            FitnessGoal.Hypertrophy => 2.1m,
            FitnessGoal.Definition => 2.2m,
            FitnessGoal.LoseWeight => 2.0m,
            _ => 1.8m
        };

        var fatRatio = fitnessProfile.Goal == FitnessGoal.Definition ? 0.24m : 0.27m;
        var protein = (int)Math.Round(fitnessProfile.WeightKg * proteinPerKg, MidpointRounding.AwayFromZero);
        var fats = (int)Math.Round((targetCalories * fatRatio) / 9m, MidpointRounding.AwayFromZero);
        var carbs = (int)Math.Round((targetCalories - (protein * 4m) - (fats * 9m)) / 4m, MidpointRounding.AwayFromZero);
        var water = decimal.Round((fitnessProfile.WeightKg * 0.035m) + Math.Min(1.2m, activitySummary.DaysTrained * 0.08m), 2, MidpointRounding.AwayFromZero);
        var meals = fitnessProfile.Goal == FitnessGoal.Hypertrophy ? 5 : 4;

        return new NutritionTargets(
            Math.Max(1400, (int)Math.Round(targetCalories, MidpointRounding.AwayFromZero)),
            Math.Max(90, protein),
            Math.Max(100, carbs),
            Math.Max(35, fats),
            meals,
            Math.Max(2.2m, water));
    }

    private static decimal ResolveActivityFactor(FitnessExperienceLevel level, int daysTrained) => (level, daysTrained) switch
    {
        (FitnessExperienceLevel.Advanced, >= 5) => 1.75m,
        (FitnessExperienceLevel.Intermediate, >= 4) => 1.6m,
        (FitnessExperienceLevel.Beginner, >= 3) => 1.5m,
        (_, >= 2) => 1.4m,
        _ => 1.25m
    };

    private static IReadOnlyCollection<MealBlueprint> BuildMealBlueprints(FitnessGoal goal, NutritionProfile profile)
    {
        var mealCalories = profile.DailyCaloriesTarget / profile.MealsPerDay;
        var breakfastCalories = (int)Math.Round(mealCalories * 0.25m, MidpointRounding.AwayFromZero);
        var lunchCalories = (int)Math.Round(mealCalories * 0.33m, MidpointRounding.AwayFromZero);
        var dinnerCalories = (int)Math.Round(mealCalories * 0.27m, MidpointRounding.AwayFromZero);
        var snackCalories = Math.Max(180, profile.DailyCaloriesTarget - breakfastCalories - lunchCalories - dinnerCalories);

        var breakfastName = goal == FitnessGoal.Hypertrophy ? "Avena proteica con yogurt y fruta" : "Huevos con avena y fruta";
        var lunchName = goal == FitnessGoal.LoseWeight ? "Pollo a la plancha con quinoa y ensalada" : "Arroz, proteína magra y vegetales";
        var dinnerName = goal == FitnessGoal.Definition ? "Pescado con vegetales y camote" : "Cena balanceada con proteína y verduras";
        var snackName = goal == FitnessGoal.Hypertrophy ? "Batido + frutos secos" : "Yogurt griego con fruta";

        return
        [
            new MealBlueprint(MealType.Breakfast, breakfastName, "Desayuno equilibrado para iniciar el día con energía.", breakfastCalories, profile.ProteinGrams / 4, profile.CarbsGrams / 4, profile.FatGrams / 4),
            new MealBlueprint(MealType.Lunch, lunchName, "Almuerzo base con proteína magra, vegetales y carbohidrato complejo.", lunchCalories, profile.ProteinGrams / 3, profile.CarbsGrams / 3, profile.FatGrams / 3),
            new MealBlueprint(MealType.Dinner, dinnerName, "Cena liviana con énfasis en saciedad y recuperación.", dinnerCalories, profile.ProteinGrams / 4, profile.CarbsGrams / 4, profile.FatGrams / 4),
            new MealBlueprint(MealType.Snack, snackName, "Snack práctico para cubrir macros del día.", snackCalories, Math.Max(15, profile.ProteinGrams - ((profile.ProteinGrams / 4) * 2 + (profile.ProteinGrams / 3))), Math.Max(20, profile.CarbsGrams - ((profile.CarbsGrams / 4) * 2 + (profile.CarbsGrams / 3))), Math.Max(8, profile.FatGrams - ((profile.FatGrams / 4) * 2 + (profile.FatGrams / 3)))),
        ];
    }

    private async Task<Customer> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.Customer) || !user.UserId.HasValue)
        {
            throw new ForbiddenException("Only customers can manage their own nutrition.");
        }

        return await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == user.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Customer profile not found.");
    }

    private async Task<Customer> GetCustomerForViewAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");
        EnsureCanViewCustomer(customer);
        return customer;
    }

    private void EnsureCanViewCustomer(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Trainer) || user.IsInRole(RoleNames.Reception)) && user.BranchId == customer.BranchId) return;
        throw new ForbiddenException("You cannot view this customer's nutrition.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static NutritionProfileResponse Map(NutritionProfile profile) => new(
        profile.Id,
        profile.CustomerId,
        profile.Goal,
        profile.DailyCaloriesTarget,
        profile.ProteinGrams,
        profile.CarbsGrams,
        profile.FatGrams,
        profile.MealsPerDay,
        profile.WaterLitersTarget,
        profile.DietaryRestrictions,
        Disclaimer,
        profile.CreatedAtUtc,
        profile.UpdatedAtUtc);

    private static MealPlanResponse Map(MealPlan plan) => new(
        plan.Id,
        plan.CustomerId,
        plan.Title,
        plan.Description,
        plan.DayOfWeek,
        plan.Items
            .OrderBy(x => x.MealType)
            .Select(x => new MealItemResponse(x.Id, x.MealType, x.Name, x.Description, x.Calories, x.ProteinGrams, x.CarbsGrams, x.FatGrams))
            .ToList(),
        plan.CreatedAtUtc);

    private sealed record BodyCompositionSnapshot(decimal WeightKg, decimal HeightCm, int Age, FitnessProfileGender Gender);
    private sealed record NutritionTargets(int DailyCaloriesTarget, int ProteinGrams, int CarbsGrams, int FatGrams, int MealsPerDay, decimal WaterLitersTarget);
    private sealed record MealBlueprint(MealType MealType, string Name, string Description, int Calories, int ProteinGrams, int CarbsGrams, int FatGrams);

    private sealed class NutritionValidationException : AppException
    {
        public NutritionValidationException(string message) : base(message)
        {
        }
    }
}
