namespace Dorian.Application.TrainingPlans;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Training.Domain.Entities;

public sealed record TrainingPlanResponse(
    Guid Id,
    Guid CustomerId,
    FitnessGoal Goal,
    FitnessExperienceLevel ExperienceLevel,
    FocusMuscleGroup FocusMuscleGroup,
    TrainingPlanStatus Status,
    DateOnly StartDate,
    DateOnly? EndDate,
    string CurrentPhaseName,
    int CompletedDaysCount,
    int TotalDaysCount,
    int ProgressPercent,
    IReadOnlyCollection<TrainingPhaseResponse> Phases);

public sealed record TrainingPhaseResponse(
    Guid Id,
    TrainingPhaseName Name,
    string Description,
    int Order,
    int DurationWeeks,
    bool IsCurrent,
    IReadOnlyCollection<TrainingWeekResponse> Weeks);

public sealed record TrainingWeekResponse(
    Guid Id,
    int WeekNumber,
    string Title,
    string Description,
    IReadOnlyCollection<TrainingPlanDayResponse> Days);

public sealed record TrainingPlanDayResponse(
    Guid Id,
    TrainingDay DayOfWeek,
    string Title,
    int EstimatedMinutes,
    TrainingDayIntensity Intensity,
    DateTimeOffset? CompletedAt,
    IReadOnlyCollection<TrainingExerciseResponse> Exercises);

public sealed record TrainingExerciseResponse(
    Guid Id,
    Guid? ExerciseId,
    string Name,
    ExerciseMuscleGroup MuscleGroup,
    int Sets,
    string Reps,
    int RestSeconds,
    decimal? WeightKg,
    string? Notes,
    int Order);

public sealed record ExerciseCatalogResponse(
    Guid Id,
    string Name,
    string Slug,
    ExerciseMuscleGroup MuscleGroup,
    ExerciseEquipment Equipment,
    string Description,
    ExerciseDifficulty Difficulty,
    string? VideoUrl,
    string? ImageUrl);
