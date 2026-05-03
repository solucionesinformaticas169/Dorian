namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class TrainingWeek : AuditableEntity<Guid>
{
    private TrainingWeek() : base(Guid.Empty)
    {
    }

    public TrainingWeek(Guid id, Guid trainingPhaseId, int weekNumber, string title, string description) : base(id)
    {
        TrainingPhaseId = trainingPhaseId;
        WeekNumber = weekNumber;
        Title = title.Trim();
        Description = description.Trim();
    }

    public Guid TrainingPhaseId { get; private set; }
    public int WeekNumber { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TrainingPhase TrainingPhase { get; private set; } = null!;
    public ICollection<TrainingPlanDay> Days { get; private set; } = [];
}
