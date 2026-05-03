namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.SharedKernel.Primitives;

public sealed class TrainingPlanDay : AuditableEntity<Guid>
{
    private TrainingPlanDay() : base(Guid.Empty)
    {
    }

    public TrainingPlanDay(Guid id, Guid trainingWeekId, TrainingDay dayOfWeek, string title, int estimatedMinutes, TrainingDayIntensity intensity) : base(id)
    {
        TrainingWeekId = trainingWeekId;
        DayOfWeek = dayOfWeek;
        Title = title.Trim();
        EstimatedMinutes = estimatedMinutes;
        Intensity = intensity;
    }

    public Guid TrainingWeekId { get; private set; }
    public TrainingDay DayOfWeek { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int EstimatedMinutes { get; private set; }
    public TrainingDayIntensity Intensity { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public TrainingWeek TrainingWeek { get; private set; } = null!;
    public ICollection<TrainingExercise> Exercises { get; private set; } = [];

    public void MarkCompleted(DateTimeOffset completedAt)
    {
        CompletedAt = completedAt;
    }

    public void MarkUncompleted()
    {
        CompletedAt = null;
    }
}
