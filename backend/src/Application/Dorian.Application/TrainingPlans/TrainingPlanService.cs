namespace Dorian.Application.TrainingPlans;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Application.WorkoutActivities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class TrainingPlanService : ITrainingPlanService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWorkoutActivityService _workoutActivityService;

    public TrainingPlanService(IDorianDbContext dbContext, ICurrentUserService currentUserService, IWorkoutActivityService workoutActivityService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _workoutActivityService = workoutActivityService;
    }

    public async Task<TrainingPlanResponse?> GetMyPlanAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var plan = await LoadLatestPlanAsync(customer.Id, cancellationToken);
        return plan is null ? null : Map(plan);
    }

    public async Task<TrainingPlanResponse?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForAdminAccessAsync(customerId, requireManage: false, cancellationToken);
        var plan = await LoadLatestPlanAsync(customer.Id, cancellationToken);
        return plan is null ? null : Map(plan);
    }

    public async Task<TrainingPlanResponse> GenerateForCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await GenerateAsync(customer, cancellationToken);
    }

    public async Task<TrainingPlanResponse> GenerateForCustomerAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForAdminAccessAsync(customerId, requireManage: true, cancellationToken);
        return await GenerateAsync(customer, cancellationToken);
    }

    public async Task<TrainingPlanDayResponse> CompleteDayAsync(Guid trainingDayId, CancellationToken cancellationToken)
    {
        var day = await _dbContext.TrainingPlanDays
            .Include(x => x.Exercises)
            .Include(x => x.TrainingWeek)
                .ThenInclude(x => x.TrainingPhase)
                    .ThenInclude(x => x.TrainingPlan)
            .SingleOrDefaultAsync(x => x.Id == trainingDayId, cancellationToken)
            ?? throw new NotFoundException("Training day not found.");

        await EnsureCanManageDayAsync(day.TrainingWeek.TrainingPhase.TrainingPlan.CustomerId, requireAdmin: false, cancellationToken);
        day.MarkCompleted(DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _workoutActivityService.EnsureAutomaticActivityForTrainingDayAsync(day.Id, cancellationToken);
        return Map(day);
    }

    public async Task<TrainingPlanDayResponse> UncompleteDayAsync(Guid trainingDayId, CancellationToken cancellationToken)
    {
        var day = await _dbContext.TrainingPlanDays
            .Include(x => x.Exercises)
            .Include(x => x.TrainingWeek)
                .ThenInclude(x => x.TrainingPhase)
                    .ThenInclude(x => x.TrainingPlan)
            .SingleOrDefaultAsync(x => x.Id == trainingDayId, cancellationToken)
            ?? throw new NotFoundException("Training day not found.");

        await EnsureCanManageDayAsync(day.TrainingWeek.TrainingPhase.TrainingPlan.CustomerId, requireAdmin: false, cancellationToken);
        day.MarkUncompleted();
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _workoutActivityService.RemoveAutomaticActivityForTrainingDayAsync(day.Id, cancellationToken);
        return Map(day);
    }

    private async Task<TrainingPlanResponse> GenerateAsync(Customer customer, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.CustomerFitnessProfiles
            .SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken)
            ?? throw new TrainingPlanValidationException("Completa tu onboarding fitness antes de generar tu plan.");

        if (!profile.OnboardingCompleted)
        {
            throw new TrainingPlanValidationException("Completa tu onboarding fitness antes de generar tu plan.");
        }

        var availableDays = profile.GetTrainingDays().Distinct().OrderBy(day => day).ToArray();
        if (availableDays.Length == 0)
        {
            throw new TrainingPlanValidationException("Debes configurar al menos un dia disponible para entrenar.");
        }

        var existingPlans = await _dbContext.TrainingPlans.Where(x => x.CustomerId == customer.Id && x.Status == TrainingPlanStatus.Active).ToListAsync(cancellationToken);
        foreach (var existing in existingPlans)
        {
            existing.Cancel(DateOnly.FromDateTime(DateTime.UtcNow));
        }

        var plan = new TrainingPlan(Guid.NewGuid(), customer.Id, profile.Goal, profile.ExperienceLevel, profile.FocusMuscleGroup, DateOnly.FromDateTime(DateTime.UtcNow));
        _dbContext.TrainingPlans.Add(plan);

        var exerciseCatalog = await _dbContext.ExerciseCatalog.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var phases = BuildPhaseBlueprints(profile.Goal);
        var dayBlueprints = BuildDayBlueprints(profile, availableDays);
        var workload = BuildWorkload(profile.Goal, profile.ExperienceLevel);

        var globalWeekNumber = 1;
        foreach (var phaseBlueprint in phases)
        {
            var phase = new TrainingPhase(Guid.NewGuid(), plan.Id, phaseBlueprint.Name, phaseBlueprint.Description, phaseBlueprint.Order, phaseBlueprint.DurationWeeks);
            _dbContext.TrainingPhases.Add(phase);

            for (var phaseWeek = 1; phaseWeek <= phaseBlueprint.DurationWeeks; phaseWeek++)
            {
                var week = new TrainingWeek(Guid.NewGuid(), phase.Id, globalWeekNumber, $"Semana {globalWeekNumber}", $"{phaseBlueprint.Description} · enfoque {phaseBlueprint.Name}.");
                _dbContext.TrainingWeeks.Add(week);

                var orderedTemplates = OrderBlueprintsByFocus(dayBlueprints, profile.FocusMuscleGroup);
                for (var dayIndex = 0; dayIndex < availableDays.Length; dayIndex++)
                {
                    var template = orderedTemplates[dayIndex % orderedTemplates.Count];
                    var day = new TrainingPlanDay(Guid.NewGuid(), week.Id, availableDays[dayIndex], template.Title, template.EstimatedMinutes + GetDurationOffset(profile.ExperienceLevel), template.Intensity);
                    _dbContext.TrainingPlanDays.Add(day);

                    var generatedExercises = BuildExercisesForDay(template, phaseBlueprint.Name, workload, exerciseCatalog, profile.Goal, profile.FocusMuscleGroup);
                    for (var exerciseOrder = 0; exerciseOrder < generatedExercises.Count; exerciseOrder++)
                    {
                        var generated = generatedExercises[exerciseOrder];
                        _dbContext.TrainingExercises.Add(new TrainingExercise(
                            Guid.NewGuid(),
                            day.Id,
                            generated.Exercise?.Id,
                            generated.Name,
                            generated.MuscleGroup,
                            generated.Sets,
                            generated.Reps,
                            generated.RestSeconds,
                            generated.WeightKg,
                            generated.Notes,
                            exerciseOrder + 1));
                    }
                }

                globalWeekNumber++;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        var created = await LoadPlanByIdAsync(plan.Id, cancellationToken) ?? throw new NotFoundException("Training plan was not created.");
        return Map(created);
    }

    private async Task<TrainingPlan?> LoadLatestPlanAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.TrainingPlans
            .AsNoTracking()
            .Include(x => x.Phases.OrderBy(phase => phase.Order))
                .ThenInclude(x => x.Weeks.OrderBy(week => week.WeekNumber))
                    .ThenInclude(x => x.Days.OrderBy(day => day.DayOfWeek))
                        .ThenInclude(x => x.Exercises.OrderBy(exercise => exercise.Order))
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<TrainingPlan?> LoadPlanByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.TrainingPlans
            .AsNoTracking()
            .Include(x => x.Phases.OrderBy(phase => phase.Order))
                .ThenInclude(x => x.Weeks.OrderBy(week => week.WeekNumber))
                    .ThenInclude(x => x.Days.OrderBy(day => day.DayOfWeek))
                        .ThenInclude(x => x.Exercises.OrderBy(exercise => exercise.Order))
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private async Task<Customer> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.Customer) || !user.UserId.HasValue)
        {
            throw new ForbiddenException("Only customers can manage their own training plan.");
        }

        return await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == user.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Customer profile not found.");
    }

    private async Task<Customer> GetCustomerForAdminAccessAsync(Guid customerId, bool requireManage, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin))
        {
            return customer;
        }

        var sameBranch = user.BranchId == customer.BranchId && (user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Trainer));
        if (sameBranch)
        {
            return customer;
        }

        if (!requireManage && user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId)
        {
            return customer;
        }

        throw new ForbiddenException("You cannot access this customer's training plan.");
    }

    private async Task EnsureCanManageDayAsync(Guid customerId, bool requireAdmin, CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin))
        {
            return;
        }

        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        if (!requireAdmin && user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId)
        {
            return;
        }

        throw new ForbiddenException("You cannot update this training day.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static IReadOnlyCollection<TrainingPhaseBlueprint> BuildPhaseBlueprints(FitnessGoal goal)
    {
        return goal switch
        {
            FitnessGoal.LoseWeight =>
            [
                new TrainingPhaseBlueprint(1, TrainingPhaseName.Resistencia, 2, "Base metabolica y tecnica para sostener volumen de trabajo."),
                new TrainingPhaseBlueprint(2, TrainingPhaseName.Fuerza, 2, "Fuerza funcional para preservar masa muscular."),
                new TrainingPhaseBlueprint(3, TrainingPhaseName.Definicion, 2, "Circuitos y densidad para acelerar gasto calorico.")
            ],
            FitnessGoal.Definition =>
            [
                new TrainingPhaseBlueprint(1, TrainingPhaseName.Resistencia, 2, "Adaptacion y control del ritmo de trabajo."),
                new TrainingPhaseBlueprint(2, TrainingPhaseName.Fuerza, 2, "Mantenimiento de fuerza con tecnica solida."),
                new TrainingPhaseBlueprint(3, TrainingPhaseName.Definicion, 3, "Combinacion de fuerza y cardio moderado.")
            ],
            FitnessGoal.Hypertrophy =>
            [
                new TrainingPhaseBlueprint(1, TrainingPhaseName.Resistencia, 2, "Acondicionamiento inicial para tolerar mayor volumen."),
                new TrainingPhaseBlueprint(2, TrainingPhaseName.Fuerza, 2, "Subida progresiva de cargas y patrones basicos."),
                new TrainingPhaseBlueprint(3, TrainingPhaseName.Hipertrofia, 4, "Mayor volumen y enfoque en tension mecanica.")
            ],
            _ =>
            [
                new TrainingPhaseBlueprint(1, TrainingPhaseName.Resistencia, 2, "Acondicionamiento general para sostener habito."),
                new TrainingPhaseBlueprint(2, TrainingPhaseName.Fuerza, 2, "Mejora de fuerza base y control tecnico."),
                new TrainingPhaseBlueprint(3, TrainingPhaseName.Hipertrofia, 2, "Trabajo balanceado para mantener masa y forma.")
            ]
        };
    }

    private static IReadOnlyList<DayBlueprint> BuildDayBlueprints(CustomerFitnessProfile profile, IReadOnlyCollection<TrainingDay> availableDays)
    {
        var count = availableDays.Count;
        var includeCardio = profile.IncludeCardio || profile.Goal is FitnessGoal.LoseWeight or FitnessGoal.Definition;

        if (count == 1)
        {
            return [new DayBlueprint("Full body Dorian", TrainingDayIntensity.Medium, 50, [ExerciseMuscleGroup.FullBody, MapFocus(profile.FocusMuscleGroup)], includeCardio)];
        }

        if (count == 2)
        {
            return
            [
                new DayBlueprint("Torso y empuje", TrainingDayIntensity.Medium, 55, [ExerciseMuscleGroup.Chest, ExerciseMuscleGroup.Back, ExerciseMuscleGroup.Shoulders], false),
                new DayBlueprint("Piernas y core", TrainingDayIntensity.Medium, 55, [ExerciseMuscleGroup.Legs, ExerciseMuscleGroup.Glutes, ExerciseMuscleGroup.Abdomen], includeCardio)
            ];
        }

        if (count == 3)
        {
            return
            [
                new DayBlueprint("Push superior", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Chest, ExerciseMuscleGroup.Shoulders, ExerciseMuscleGroup.Triceps], false),
                new DayBlueprint("Pull superior", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Back, ExerciseMuscleGroup.Biceps, ExerciseMuscleGroup.Abdomen], false),
                new DayBlueprint("Piernas y condicionamiento", TrainingDayIntensity.High, 65, [ExerciseMuscleGroup.Legs, ExerciseMuscleGroup.Glutes, ExerciseMuscleGroup.Cardio], includeCardio)
            ];
        }

        if (count == 4)
        {
            return
            [
                new DayBlueprint("Pecho y triceps", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Chest, ExerciseMuscleGroup.Triceps], false),
                new DayBlueprint("Espalda y biceps", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Back, ExerciseMuscleGroup.Biceps], false),
                new DayBlueprint("Piernas y gluteos", TrainingDayIntensity.High, 70, [ExerciseMuscleGroup.Legs, ExerciseMuscleGroup.Glutes], false),
                new DayBlueprint("Hombros, abdomen y cardio", TrainingDayIntensity.Medium, 55, [ExerciseMuscleGroup.Shoulders, ExerciseMuscleGroup.Abdomen, ExerciseMuscleGroup.Cardio], includeCardio)
            ];
        }

        return
        [
            new DayBlueprint("Pecho y triceps", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Chest, ExerciseMuscleGroup.Triceps], false),
            new DayBlueprint("Espalda y biceps", TrainingDayIntensity.Medium, 60, [ExerciseMuscleGroup.Back, ExerciseMuscleGroup.Biceps], false),
            new DayBlueprint("Piernas", TrainingDayIntensity.High, 70, [ExerciseMuscleGroup.Legs, ExerciseMuscleGroup.Glutes], false),
            new DayBlueprint("Hombros y abdomen", TrainingDayIntensity.Medium, 55, [ExerciseMuscleGroup.Shoulders, ExerciseMuscleGroup.Abdomen], false),
            new DayBlueprint("Full body metabolico", TrainingDayIntensity.High, 50, [ExerciseMuscleGroup.FullBody, ExerciseMuscleGroup.Cardio], includeCardio)
        ];
    }

    private static IReadOnlyList<DayBlueprint> OrderBlueprintsByFocus(IReadOnlyList<DayBlueprint> templates, FocusMuscleGroup focus)
    {
        if (focus == FocusMuscleGroup.Balanced)
        {
            return templates;
        }

        var preferredGroup = MapFocus(focus);
        var ordered = templates.OrderByDescending(x => x.MuscleGroups.Contains(preferredGroup)).ToList();
        return ordered;
    }

    private static WorkloadProfile BuildWorkload(FitnessGoal goal, FitnessExperienceLevel level)
    {
        return (goal, level) switch
        {
            (FitnessGoal.Hypertrophy, FitnessExperienceLevel.Beginner) => new WorkloadProfile(3, "10-12", 75),
            (FitnessGoal.Hypertrophy, FitnessExperienceLevel.Intermediate) => new WorkloadProfile(4, "8-12", 75),
            (FitnessGoal.Hypertrophy, FitnessExperienceLevel.Advanced) => new WorkloadProfile(5, "6-10", 90),
            (FitnessGoal.LoseWeight, FitnessExperienceLevel.Beginner) => new WorkloadProfile(2, "12-15", 45),
            (FitnessGoal.LoseWeight, FitnessExperienceLevel.Intermediate) => new WorkloadProfile(3, "12-15", 45),
            (FitnessGoal.LoseWeight, FitnessExperienceLevel.Advanced) => new WorkloadProfile(4, "10-15", 45),
            (FitnessGoal.Definition, FitnessExperienceLevel.Beginner) => new WorkloadProfile(3, "12-15", 50),
            (FitnessGoal.Definition, FitnessExperienceLevel.Intermediate) => new WorkloadProfile(4, "10-14", 50),
            (FitnessGoal.Definition, FitnessExperienceLevel.Advanced) => new WorkloadProfile(4, "8-12", 60),
            (_, FitnessExperienceLevel.Beginner) => new WorkloadProfile(2, "12-15", 60),
            (_, FitnessExperienceLevel.Intermediate) => new WorkloadProfile(3, "10-12", 60),
            _ => new WorkloadProfile(4, "8-10", 75)
        };
    }

    private static int GetDurationOffset(FitnessExperienceLevel level) => level switch
    {
        FitnessExperienceLevel.Beginner => 0,
        FitnessExperienceLevel.Intermediate => 8,
        _ => 15
    };

    private static IReadOnlyList<GeneratedExercise> BuildExercisesForDay(
        DayBlueprint template,
        TrainingPhaseName phase,
        WorkloadProfile workload,
        IReadOnlyCollection<ExerciseCatalog> exerciseCatalog,
        FitnessGoal goal,
        FocusMuscleGroup focus)
    {
        var exercises = new List<GeneratedExercise>();
        var distinctGroups = template.MuscleGroups.Distinct().ToList();
        var rest = phase switch
        {
            TrainingPhaseName.Fuerza => workload.RestSeconds + 20,
            TrainingPhaseName.Hipertrofia => workload.RestSeconds + 10,
            TrainingPhaseName.Definicion => Math.Max(30, workload.RestSeconds - 10),
            _ => workload.RestSeconds
        };

        foreach (var group in distinctGroups)
        {
            var pool = exerciseCatalog.Where(x => x.MuscleGroup == group || (group == ExerciseMuscleGroup.FullBody && x.MuscleGroup == ExerciseMuscleGroup.FullBody)).OrderBy(x => x.Name).ToList();
            if (pool.Count == 0) continue;

            var take = group == ExerciseMuscleGroup.Cardio ? 1 : 2;
            foreach (var exercise in pool.Take(take))
            {
                var notes = focus != FocusMuscleGroup.Balanced && group == MapFocus(focus) ? "Prioriza tecnica y control en este ejercicio foco." : null;
                exercises.Add(new GeneratedExercise(exercise, exercise.Name, exercise.MuscleGroup, workload.Sets, workload.Reps, rest, null, notes));
            }
        }

        if ((goal == FitnessGoal.LoseWeight || goal == FitnessGoal.Definition || template.IncludeCardio) && exercises.All(x => x.MuscleGroup != ExerciseMuscleGroup.Cardio))
        {
            var cardio = exerciseCatalog.First(x => x.MuscleGroup == ExerciseMuscleGroup.Cardio);
            exercises.Add(new GeneratedExercise(cardio, cardio.Name, cardio.MuscleGroup, 1, "12-20 min", 30, null, "Cardio de cierre para elevar el gasto energetico."));
        }

        return exercises;
    }

    private static ExerciseMuscleGroup MapFocus(FocusMuscleGroup focus) => focus switch
    {
        FocusMuscleGroup.Chest => ExerciseMuscleGroup.Chest,
        FocusMuscleGroup.Back => ExerciseMuscleGroup.Back,
        FocusMuscleGroup.Arms => ExerciseMuscleGroup.Biceps,
        FocusMuscleGroup.Legs => ExerciseMuscleGroup.Legs,
        FocusMuscleGroup.Abs => ExerciseMuscleGroup.Abdomen,
        FocusMuscleGroup.Glutes => ExerciseMuscleGroup.Glutes,
        _ => ExerciseMuscleGroup.FullBody
    };

    private static TrainingPlanResponse Map(TrainingPlan plan)
    {
        var orderedPhases = plan.Phases.OrderBy(x => x.Order).ToList();
        var flattenedDays = orderedPhases.SelectMany(x => x.Weeks).SelectMany(x => x.Days).OrderBy(x => x.CompletedAt.HasValue).ThenBy(x => x.DayOfWeek).ToList();
        var currentPhase = orderedPhases.FirstOrDefault(phase => phase.Weeks.SelectMany(week => week.Days).Any(day => !day.CompletedAt.HasValue)) ?? orderedPhases.LastOrDefault();
        var totalDays = flattenedDays.Count;
        var completedDays = flattenedDays.Count(x => x.CompletedAt.HasValue);
        var progress = totalDays == 0 ? 0 : (int)Math.Round((completedDays / (decimal)totalDays) * 100, MidpointRounding.AwayFromZero);

        return new TrainingPlanResponse(
            plan.Id,
            plan.CustomerId,
            plan.Goal,
            plan.ExperienceLevel,
            plan.FocusMuscleGroup,
            plan.Status,
            plan.StartDate,
            plan.EndDate,
            currentPhase?.Name.ToString() ?? "Sin fases",
            completedDays,
            totalDays,
            progress,
            orderedPhases.Select(phase => new TrainingPhaseResponse(
                phase.Id,
                phase.Name,
                phase.Description,
                phase.Order,
                phase.DurationWeeks,
                phase.Id == currentPhase?.Id,
                phase.Weeks.OrderBy(week => week.WeekNumber).Select(week => new TrainingWeekResponse(
                    week.Id,
                    week.WeekNumber,
                    week.Title,
                    week.Description,
                    week.Days.OrderBy(day => day.DayOfWeek).Select(Map).ToList())).ToList())).ToList());
    }

    private static TrainingPlanDayResponse Map(TrainingPlanDay day) => new(
        day.Id,
        day.DayOfWeek,
        day.Title,
        day.EstimatedMinutes,
        day.Intensity,
        day.CompletedAt,
        day.Exercises.OrderBy(exercise => exercise.Order).Select(exercise => new TrainingExerciseResponse(
            exercise.Id,
            exercise.ExerciseId,
            exercise.Name,
            exercise.MuscleGroup,
            exercise.Sets,
            exercise.Reps,
            exercise.RestSeconds,
            exercise.WeightKg,
            exercise.Notes,
            exercise.Order)).ToList());

    private sealed record TrainingPhaseBlueprint(int Order, TrainingPhaseName Name, int DurationWeeks, string Description);
    private sealed record DayBlueprint(string Title, TrainingDayIntensity Intensity, int EstimatedMinutes, IReadOnlyCollection<ExerciseMuscleGroup> MuscleGroups, bool IncludeCardio);
    private sealed record WorkloadProfile(int Sets, string Reps, int RestSeconds);
    private sealed record GeneratedExercise(ExerciseCatalog? Exercise, string Name, ExerciseMuscleGroup MuscleGroup, int Sets, string Reps, int RestSeconds, decimal? WeightKg, string? Notes);

    private sealed class TrainingPlanValidationException : AppException
    {
        public TrainingPlanValidationException(string message) : base(message)
        {
        }
    }
}
