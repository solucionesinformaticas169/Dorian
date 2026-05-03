namespace Dorian.Modules.Customers.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class CustomerFitnessProfile : AuditableEntity<Guid>
{
    private CustomerFitnessProfile() : base(Guid.Empty)
    {
    }

    public CustomerFitnessProfile(
        Guid id,
        Guid customerId,
        FitnessGoal goal,
        FocusMuscleGroup focusMuscleGroup,
        FitnessExperienceLevel experienceLevel,
        GymType gymType,
        bool includeCardio,
        IReadOnlyCollection<TrainingDay> trainingDays,
        string? preferredTrainingTime,
        FitnessProfileGender gender,
        DateOnly birthDate,
        decimal weightKg,
        decimal heightCm,
        decimal targetWeightKg,
        bool notificationsEnabled,
        NotificationIntensity notificationIntensity,
        bool onboardingCompleted) : base(id)
    {
        CustomerId = customerId;
        Update(
            goal,
            focusMuscleGroup,
            experienceLevel,
            gymType,
            includeCardio,
            trainingDays,
            preferredTrainingTime,
            gender,
            birthDate,
            weightKg,
            heightCm,
            targetWeightKg,
            notificationsEnabled,
            notificationIntensity,
            onboardingCompleted);
    }

    public Guid CustomerId { get; private set; }
    public FitnessGoal Goal { get; private set; }
    public FocusMuscleGroup FocusMuscleGroup { get; private set; }
    public FitnessExperienceLevel ExperienceLevel { get; private set; }
    public GymType GymType { get; private set; }
    public bool IncludeCardio { get; private set; }
    public string TrainingDaysSerialized { get; private set; } = string.Empty;
    public string? PreferredTrainingTime { get; private set; }
    public FitnessProfileGender Gender { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public decimal WeightKg { get; private set; }
    public decimal HeightCm { get; private set; }
    public decimal TargetWeightKg { get; private set; }
    public bool NotificationsEnabled { get; private set; }
    public NotificationIntensity NotificationIntensity { get; private set; }
    public bool OnboardingCompleted { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public IReadOnlyCollection<TrainingDay> GetTrainingDays()
    {
        if (string.IsNullOrWhiteSpace(TrainingDaysSerialized))
        {
            return [];
        }

        return TrainingDaysSerialized
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => Enum.Parse<TrainingDay>(value, ignoreCase: true))
            .ToArray();
    }

    public void Update(
        FitnessGoal goal,
        FocusMuscleGroup focusMuscleGroup,
        FitnessExperienceLevel experienceLevel,
        GymType gymType,
        bool includeCardio,
        IReadOnlyCollection<TrainingDay> trainingDays,
        string? preferredTrainingTime,
        FitnessProfileGender gender,
        DateOnly birthDate,
        decimal weightKg,
        decimal heightCm,
        decimal targetWeightKg,
        bool notificationsEnabled,
        NotificationIntensity notificationIntensity,
        bool onboardingCompleted)
    {
        Goal = goal;
        FocusMuscleGroup = focusMuscleGroup;
        ExperienceLevel = experienceLevel;
        GymType = gymType;
        IncludeCardio = includeCardio;
        TrainingDaysSerialized = string.Join(',', trainingDays.Distinct().OrderBy(day => day));
        PreferredTrainingTime = string.IsNullOrWhiteSpace(preferredTrainingTime) ? null : preferredTrainingTime.Trim();
        Gender = gender;
        BirthDate = birthDate;
        WeightKg = decimal.Round(weightKg, 2);
        HeightCm = decimal.Round(heightCm, 2);
        TargetWeightKg = decimal.Round(targetWeightKg, 2);
        NotificationsEnabled = notificationsEnabled;
        NotificationIntensity = notificationIntensity;
        OnboardingCompleted = onboardingCompleted;
    }
}
