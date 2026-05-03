namespace Dorian.Application.CustomerFitnessProfiles;

using Dorian.Modules.Customers.Domain.Entities;

public sealed record CustomerFitnessProfileResponse(
    Guid? Id,
    Guid? CustomerId,
    FitnessGoal? Goal,
    FocusMuscleGroup? FocusMuscleGroup,
    FitnessExperienceLevel? ExperienceLevel,
    GymType? GymType,
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
    DateTimeOffset? UpdatedAtUtc);

public sealed record SaveCustomerFitnessProfileRequest(
    FitnessGoal Goal,
    FocusMuscleGroup FocusMuscleGroup,
    FitnessExperienceLevel ExperienceLevel,
    GymType GymType,
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
    bool OnboardingCompleted);
