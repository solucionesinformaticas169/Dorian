namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class TrainingPhase : AuditableEntity<Guid>
{
    private TrainingPhase() : base(Guid.Empty)
    {
    }

    public TrainingPhase(Guid id, Guid trainingPlanId, TrainingPhaseName name, string description, int order, int durationWeeks) : base(id)
    {
        TrainingPlanId = trainingPlanId;
        Name = name;
        Description = description.Trim();
        Order = order;
        DurationWeeks = durationWeeks;
    }

    public Guid TrainingPlanId { get; private set; }
    public TrainingPhaseName Name { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public int DurationWeeks { get; private set; }
    public TrainingPlan TrainingPlan { get; private set; } = null!;
    public ICollection<TrainingWeek> Weeks { get; private set; } = [];
}
