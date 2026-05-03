namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class WorkoutActivity : AuditableEntity<Guid>
{
    private WorkoutActivity() : base(Guid.Empty)
    {
    }

    public WorkoutActivity(
        Guid id,
        Guid customerId,
        Guid? trainingDayId,
        DateTimeOffset completedAt,
        int durationSeconds,
        int caloriesEstimated,
        string? notes) : base(id)
    {
        CustomerId = customerId;
        TrainingDayId = trainingDayId;
        CompletedAt = completedAt;
        DurationSeconds = durationSeconds;
        CaloriesEstimated = caloriesEstimated;
        Notes = Normalize(notes);
    }

    public Guid CustomerId { get; private set; }
    public Guid? TrainingDayId { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }
    public int DurationSeconds { get; private set; }
    public int CaloriesEstimated { get; private set; }
    public string? Notes { get; private set; }
    public TrainingPlanDay? TrainingDay { get; private set; }
    public ICollection<WorkoutExerciseLog> ExerciseLogs { get; private set; } = [];

    public void Update(DateTimeOffset completedAt, int durationSeconds, int caloriesEstimated, string? notes)
    {
        CompletedAt = completedAt;
        DurationSeconds = durationSeconds;
        CaloriesEstimated = caloriesEstimated;
        Notes = Normalize(notes);
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
