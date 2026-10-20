namespace Dorian.Application.CustomerFitnessProfiles;

using Dorian.Modules.Customers.Domain.Entities;

public sealed record CustomerFitnessProfileResponse(
    Guid? Id,
    Guid? CustomerId,
    FitnessGoal? Goal,
    IReadOnlyCollection<PriorityMuscleGroup> PriorityMuscleGroups,
    FitnessExperienceLevel? ExperienceLevel,
    bool IncludeCardio,
    IReadOnlyCollection<TrainingDay> TrainingDays,
    string? PreferredTrainingTime,
    FitnessProfileGender? Gender,
    DateOnly? BirthDate,
    decimal? WeightKg,
    decimal? HeightCm,
    decimal? TargetWeightKg,
    bool NotificationsEnabled,
    NotificationIntensity? NotificationIntensity,
    bool OnboardingCompleted,
    DateTimeOffset? CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    FocusMuscleGroup? FocusMuscleGroup = null,
    GymType? GymType = null);

public sealed record SaveCustomerFitnessProfileRequest(
    FitnessGoal Goal,
    IReadOnlyCollection<PriorityMuscleGroup>? PriorityMuscleGroups,
    FitnessExperienceLevel ExperienceLevel,
    bool IncludeCardio,
    IReadOnlyCollection<TrainingDay> TrainingDays,
    string? PreferredTrainingTime,
    FitnessProfileGender Gender,
    DateOnly BirthDate,
    decimal WeightKg,
    decimal HeightCm,
    decimal TargetWeightKg,
    bool NotificationsEnabled,
    NotificationIntensity NotificationIntensity,
    bool OnboardingCompleted,
    FocusMuscleGroup? FocusMuscleGroup = null,
    GymType? GymType = null);
