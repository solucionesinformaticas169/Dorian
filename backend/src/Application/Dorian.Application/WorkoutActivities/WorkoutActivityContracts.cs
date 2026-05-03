namespace Dorian.Application.WorkoutActivities;

using Dorian.Modules.Training.Domain.Entities;

public sealed record CreateWorkoutActivityRequest(
    DateTimeOffset CompletedAt,
    int DurationSeconds,
    int CaloriesEstimated,
    string? Notes,
    IReadOnlyCollection<CreateWorkoutExerciseLogRequest> Exercises);

public sealed record CreateWorkoutExerciseLogRequest(
    string ExerciseName,
    ExerciseMuscleGroup MuscleGroup,
    int Sets,
    string Reps,
    decimal? WeightKg,
    bool Completed);

public sealed record WorkoutActivityResponse(
    Guid Id,
    Guid CustomerId,
    Guid? TrainingDayId,
    string Title,
    DateTimeOffset CompletedAt,
    int DurationSeconds,
    int CaloriesEstimated,
    string? Notes,
    int ExercisesCompleted,
    IReadOnlyCollection<string> MuscleGroups);

public sealed record ActivityByDayPointResponse(
    DateOnly Day,
    int ActivityCount,
    int DurationSeconds,
    int CaloriesEstimated,
    int ExercisesCompleted);

public sealed record MuscleActivityResponse(
    string MuscleGroup,
    int Sessions,
    int ExercisesCompleted,
    int Percentage,
    string FatigueStatus);

public sealed record ActivityHistoryItemResponse(
    Guid Id,
    string Title,
    DateTimeOffset CompletedAt,
    int DurationSeconds,
    int CaloriesEstimated,
    int ExercisesCompleted,
    IReadOnlyCollection<string> MuscleGroups,
    string? Notes);

public sealed record ActivitySummaryResponse(
    int RangeInDays,
    int DaysTrained,
    int TotalDurationSeconds,
    int CaloriesEstimated,
    int ExercisesCompleted,
    int SeriesCompleted,
    int RepsCompleted,
    decimal? TotalLoadKg,
    IReadOnlyCollection<MuscleActivityResponse> MuscleGroups,
    IReadOnlyCollection<ActivityByDayPointResponse> ActivityByDay,
    IReadOnlyCollection<ActivityHistoryItemResponse> RecentActivities);
