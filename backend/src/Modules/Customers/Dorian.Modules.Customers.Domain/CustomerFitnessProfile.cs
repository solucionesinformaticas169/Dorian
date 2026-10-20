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
        IReadOnlyCollection<PriorityMuscleGroup> priorityMuscleGroups,
        FitnessExperienceLevel experienceLevel,
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
            priorityMuscleGroups,
            experienceLevel,
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
    public string PriorityMuscleGroupsSerialized { get; private set; } = string.Empty;
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

    public static IReadOnlyCollection<PriorityMuscleGroup> FromLegacyFocus(FocusMuscleGroup? focus) => focus switch
    {
        FocusMuscleGroup.Chest => [PriorityMuscleGroup.Chest],
        FocusMuscleGroup.Back => [PriorityMuscleGroup.Back],
        FocusMuscleGroup.Arms => [PriorityMuscleGroup.Biceps, PriorityMuscleGroup.Triceps],
        FocusMuscleGroup.Legs => [PriorityMuscleGroup.Quadriceps],
        FocusMuscleGroup.Glutes => [PriorityMuscleGroup.Glutes],
        _ => []
    };

    public void ApplyLegacySettings(FocusMuscleGroup? focus, GymType? gymType)
    {
        if (focus.HasValue) FocusMuscleGroup = focus.Value;
        if (gymType.HasValue) GymType = gymType.Value;
    }

    public IReadOnlyCollection<PriorityMuscleGroup> GetPriorityMuscleGroups()
    {
        if (string.IsNullOrWhiteSpace(PriorityMuscleGroupsSerialized))
        {
            return FromLegacyFocus(FocusMuscleGroup);
        }

        return PriorityMuscleGroupsSerialized
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => Enum.Parse<PriorityMuscleGroup>(value, ignoreCase: true))
            .ToArray();
    }

    public void Update(
        FitnessGoal goal,
        IReadOnlyCollection<PriorityMuscleGroup> priorityMuscleGroups,
        FitnessExperienceLevel experienceLevel,
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
        var sanitizedPriorityMuscleGroups = priorityMuscleGroups
            .Distinct()
            .OrderBy(group => group)
            .ToArray();

        Goal = goal;
        PriorityMuscleGroupsSerialized = string.Join(',', sanitizedPriorityMuscleGroups);
        FocusMuscleGroup = MapLegacyFocus(sanitizedPriorityMuscleGroups);
        ExperienceLevel = experienceLevel;
        GymType = GymType.Advanced;
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

    private static FocusMuscleGroup MapLegacyFocus(IReadOnlyCollection<PriorityMuscleGroup> priorityMuscleGroups)
    {
        if (priorityMuscleGroups.Count == 0)
        {
            return FocusMuscleGroup.Balanced;
        }

        if (priorityMuscleGroups.Any(group => group is PriorityMuscleGroup.Chest))
        {
            return FocusMuscleGroup.Chest;
        }

        if (priorityMuscleGroups.Any(group => group is PriorityMuscleGroup.Back))
        {
            return FocusMuscleGroup.Back;
        }

        if (priorityMuscleGroups.Any(group => group is PriorityMuscleGroup.Biceps or PriorityMuscleGroup.Triceps or PriorityMuscleGroup.Forearm))
        {
            return FocusMuscleGroup.Arms;
        }

        if (priorityMuscleGroups.Any(group => group is PriorityMuscleGroup.Glutes))
        {
            return FocusMuscleGroup.Glutes;
        }

        if (priorityMuscleGroups.Any(group => group is PriorityMuscleGroup.Quadriceps or PriorityMuscleGroup.Adductors or PriorityMuscleGroup.Abduction or PriorityMuscleGroup.Calves or PriorityMuscleGroup.Tibials))
        {
            return FocusMuscleGroup.Legs;
        }

        return FocusMuscleGroup.Balanced;
    }
}
