namespace Dorian.Application.WorkoutActivities;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class WorkoutActivityService : IWorkoutActivityService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public WorkoutActivityService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ActivitySummaryResponse> GetMySummaryAsync(int rangeInDays, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await BuildSummaryAsync(customer, NormalizeRange(rangeInDays), cancellationToken);
    }

    public async Task<ActivitySummaryResponse> GetSummaryByCustomerIdAsync(Guid customerId, int rangeInDays, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForViewAsync(customerId, cancellationToken);
        return await BuildSummaryAsync(customer, NormalizeRange(rangeInDays), cancellationToken);
    }

    public async Task<IReadOnlyCollection<ActivityHistoryItemResponse>> GetMyHistoryAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await GetHistoryForCustomerAsync(customer.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MuscleActivityResponse>> GetMyMuscleActivityAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var activities = await LoadActivitiesAsync(customer.Id, null, cancellationToken);
        return BuildMuscleActivity(activities);
    }

    public async Task<WorkoutActivityResponse> CreateManualActivityAsync(CreateWorkoutActivityRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        ValidateRequest(request);

        var activity = new WorkoutActivity(
            Guid.NewGuid(),
            customer.Id,
            null,
            request.CompletedAt,
            request.DurationSeconds,
            request.CaloriesEstimated,
            request.Notes);

        foreach (var exercise in request.Exercises)
        {
            activity.ExerciseLogs.Add(new WorkoutExerciseLog(
                Guid.NewGuid(),
                activity.Id,
                exercise.ExerciseName,
                exercise.MuscleGroup,
                exercise.Sets,
                exercise.Reps,
                exercise.WeightKg,
                exercise.Completed));
        }

        _dbContext.WorkoutActivities.Add(activity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetActivityByIdAsync(activity.Id, cancellationToken);
    }

    public async Task EnsureAutomaticActivityForTrainingDayAsync(Guid trainingDayId, CancellationToken cancellationToken)
    {
        var day = await _dbContext.TrainingPlanDays
            .Include(x => x.Exercises)
            .Include(x => x.TrainingWeek)
                .ThenInclude(x => x.TrainingPhase)
                    .ThenInclude(x => x.TrainingPlan)
            .SingleOrDefaultAsync(x => x.Id == trainingDayId, cancellationToken)
            ?? throw new NotFoundException("Training day not found.");

        if (!day.CompletedAt.HasValue)
        {
            return;
        }

        var existing = await _dbContext.WorkoutActivities
            .Include(x => x.ExerciseLogs)
            .SingleOrDefaultAsync(x => x.TrainingDayId == trainingDayId, cancellationToken);

        var calories = EstimateCalories(day.EstimatedMinutes, day.Intensity);
        var title = day.Title;

        if (existing is null)
        {
            var activity = new WorkoutActivity(
                Guid.NewGuid(),
                day.TrainingWeek.TrainingPhase.TrainingPlan.CustomerId,
                day.Id,
                day.CompletedAt.Value,
                day.EstimatedMinutes * 60,
                calories,
                $"Actividad generada automaticamente desde el plan: {title}.");

            foreach (var exercise in day.Exercises.OrderBy(x => x.Order))
            {
                activity.ExerciseLogs.Add(new WorkoutExerciseLog(
                    Guid.NewGuid(),
                    activity.Id,
                    exercise.Name,
                    exercise.MuscleGroup,
                    exercise.Sets,
                    exercise.Reps,
                    exercise.WeightKg,
                    true));
            }

            _dbContext.WorkoutActivities.Add(activity);
        }
        else
        {
            existing.Update(day.CompletedAt.Value, day.EstimatedMinutes * 60, calories, $"Actividad generada automaticamente desde el plan: {title}.");
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAutomaticActivityForTrainingDayAsync(Guid trainingDayId, CancellationToken cancellationToken)
    {
        var activity = await _dbContext.WorkoutActivities
            .SingleOrDefaultAsync(x => x.TrainingDayId == trainingDayId, cancellationToken);

        if (activity is null)
        {
            return;
        }

        _dbContext.WorkoutActivities.Remove(activity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<ActivitySummaryResponse> BuildSummaryAsync(Customer customer, int rangeInDays, CancellationToken cancellationToken)
    {
        var activities = await LoadActivitiesAsync(customer.Id, rangeInDays, cancellationToken);
        var recent = activities
            .OrderByDescending(x => x.CompletedAt)
            .Take(5)
            .Select(MapHistory)
            .ToList();

        var daysTrained = activities
            .Select(x => DateOnly.FromDateTime(x.CompletedAt.UtcDateTime.Date))
            .Distinct()
            .Count();

        var exerciseLogs = activities.SelectMany(x => x.ExerciseLogs).ToList();
        var seriesCompleted = exerciseLogs.Where(x => x.Completed).Sum(x => x.Sets);
        var repsCompleted = exerciseLogs.Where(x => x.Completed).Sum(CalculateCompletedReps);
        var totalLoad = exerciseLogs.Where(x => x.Completed && x.WeightKg.HasValue).Sum(x => x.WeightKg!.Value * x.Sets);

        return new ActivitySummaryResponse(
            rangeInDays,
            daysTrained,
            activities.Sum(x => x.DurationSeconds),
            activities.Sum(x => x.CaloriesEstimated),
            exerciseLogs.Count(x => x.Completed),
            seriesCompleted,
            repsCompleted,
            totalLoad == 0 ? null : Math.Round(totalLoad, 2, MidpointRounding.AwayFromZero),
            BuildMuscleActivity(activities),
            BuildActivityByDay(activities, rangeInDays),
            recent);
    }

    private async Task<IReadOnlyCollection<ActivityHistoryItemResponse>> GetHistoryForCustomerAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var activities = await LoadActivitiesAsync(customerId, null, cancellationToken);
        return activities.OrderByDescending(x => x.CompletedAt).Select(MapHistory).ToList();
    }

    private async Task<List<WorkoutActivity>> LoadActivitiesAsync(Guid customerId, int? rangeInDays, CancellationToken cancellationToken)
    {
        var query = _dbContext.WorkoutActivities
            .AsNoTracking()
            .Include(x => x.ExerciseLogs)
            .Include(x => x.TrainingDay)
            .Where(x => x.CustomerId == customerId);

        if (rangeInDays.HasValue)
        {
            var from = DateTimeOffset.UtcNow.Date.AddDays(-(rangeInDays.Value - 1));
            query = query.Where(x => x.CompletedAt >= from);
        }

        return await query
            .OrderByDescending(x => x.CompletedAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    private async Task<WorkoutActivityResponse> GetActivityByIdAsync(Guid activityId, CancellationToken cancellationToken)
    {
        var activity = await _dbContext.WorkoutActivities
            .AsNoTracking()
            .Include(x => x.ExerciseLogs)
            .Include(x => x.TrainingDay)
            .SingleAsync(x => x.Id == activityId, cancellationToken);

        return Map(activity);
    }

    private async Task<Customer> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.Customer) || !user.UserId.HasValue)
        {
            throw new ForbiddenException("Only customers can manage their own activity.");
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
        throw new ForbiddenException("You cannot view this customer's activity.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static void ValidateRequest(CreateWorkoutActivityRequest request)
    {
        if (request.DurationSeconds <= 0) throw new WorkoutActivityValidationException("La duracion debe ser mayor a cero.");
        if (request.CaloriesEstimated < 0) throw new WorkoutActivityValidationException("Las calorias no pueden ser negativas.");
        if (request.CompletedAt > DateTimeOffset.UtcNow.AddMinutes(1)) throw new WorkoutActivityValidationException("La fecha de actividad no puede ser futura.");
        foreach (var exercise in request.Exercises)
        {
            if (string.IsNullOrWhiteSpace(exercise.ExerciseName)) throw new WorkoutActivityValidationException("Cada ejercicio debe tener nombre.");
            if (exercise.Sets < 0) throw new WorkoutActivityValidationException("Las series no pueden ser negativas.");
        }
    }

    private static int NormalizeRange(int rangeInDays) => rangeInDays switch
    {
        7 or 14 or 28 or 90 => rangeInDays,
        _ => 7
    };

    private static int EstimateCalories(int estimatedMinutes, TrainingDayIntensity intensity)
    {
        var factor = intensity switch
        {
            TrainingDayIntensity.Low => 6,
            TrainingDayIntensity.Medium => 8,
            _ => 10
        };

        return estimatedMinutes * factor;
    }

    private static IReadOnlyCollection<ActivityByDayPointResponse> BuildActivityByDay(IEnumerable<WorkoutActivity> activities, int rangeInDays)
    {
        var from = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-(rangeInDays - 1)));
        var grouped = activities
            .GroupBy(x => DateOnly.FromDateTime(x.CompletedAt.UtcDateTime.Date))
            .ToDictionary(
                x => x.Key,
                x => new ActivityByDayPointResponse(
                    x.Key,
                    x.Count(),
                    x.Sum(y => y.DurationSeconds),
                    x.Sum(y => y.CaloriesEstimated),
                    x.SelectMany(y => y.ExerciseLogs).Count(y => y.Completed)));

        var points = new List<ActivityByDayPointResponse>();
        for (var index = 0; index < rangeInDays; index++)
        {
            var day = from.AddDays(index);
            points.Add(grouped.TryGetValue(day, out var point)
                ? point
                : new ActivityByDayPointResponse(day, 0, 0, 0, 0));
        }

        return points;
    }

    private static IReadOnlyCollection<MuscleActivityResponse> BuildMuscleActivity(IEnumerable<WorkoutActivity> activities)
    {
        var logs = activities.SelectMany(x => x.ExerciseLogs.Where(y => y.Completed)).ToList();
        var total = logs.Count;
        if (total == 0)
        {
            return [];
        }

        var cutoffFatigued = DateTimeOffset.UtcNow.AddDays(-1);
        var cutoffRecovery = DateTimeOffset.UtcNow.AddDays(-3);
        var cutoffWeak = DateTimeOffset.UtcNow.AddDays(-6);

        return activities
            .SelectMany(activity => activity.ExerciseLogs.Where(log => log.Completed).Select(log => new { activity.CompletedAt, Log = log }))
            .GroupBy(x => x.Log.MuscleGroup)
            .OrderByDescending(x => x.Count())
            .Select(group =>
            {
                var latest = group.Max(x => x.CompletedAt);
                var fatigue = latest >= cutoffFatigued
                    ? "Fatigado"
                    : latest >= cutoffRecovery
                        ? "En recuperacion"
                        : latest >= cutoffWeak
                            ? "Debilitado"
                            : "Recuperado";

                return new MuscleActivityResponse(
                    group.Key.ToString(),
                    group.Select(x => DateOnly.FromDateTime(x.CompletedAt.UtcDateTime.Date)).Distinct().Count(),
                    group.Count(),
                    (int)Math.Round((group.Count() / (decimal)total) * 100, MidpointRounding.AwayFromZero),
                    fatigue);
            })
            .ToList();
    }

    private static ActivityHistoryItemResponse MapHistory(WorkoutActivity activity) => new(
        activity.Id,
        activity.TrainingDay?.Title ?? "Actividad manual",
        activity.CompletedAt,
        activity.DurationSeconds,
        activity.CaloriesEstimated,
        activity.ExerciseLogs.Count(x => x.Completed),
        activity.ExerciseLogs.Where(x => x.Completed).Select(x => x.MuscleGroup.ToString()).Distinct().ToList(),
        activity.Notes);

    private static WorkoutActivityResponse Map(WorkoutActivity activity) => new(
        activity.Id,
        activity.CustomerId,
        activity.TrainingDayId,
        activity.TrainingDay?.Title ?? "Actividad manual",
        activity.CompletedAt,
        activity.DurationSeconds,
        activity.CaloriesEstimated,
        activity.Notes,
        activity.ExerciseLogs.Count(x => x.Completed),
        activity.ExerciseLogs.Where(x => x.Completed).Select(x => x.MuscleGroup.ToString()).Distinct().ToList());

    private static int CalculateCompletedReps(WorkoutExerciseLog log)
    {
        if (string.IsNullOrWhiteSpace(log.Reps))
        {
            return 0;
        }

        var digits = new string(log.Reps.TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(digits, out var reps) ? reps * Math.Max(log.Sets, 1) : 0;
    }

    private sealed class WorkoutActivityValidationException : AppException
    {
        public WorkoutActivityValidationException(string message) : base(message)
        {
        }
    }
}
