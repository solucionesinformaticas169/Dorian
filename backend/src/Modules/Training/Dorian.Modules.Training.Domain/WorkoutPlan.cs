namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class WorkoutPlan : AuditableEntity<Guid>
{
    public WorkoutPlan(Guid id, Guid clientUserId, Guid trainerUserId, string title) : base(id)
    {
        ClientUserId = clientUserId;
        TrainerUserId = trainerUserId;
        Title = title;
    }

    public Guid ClientUserId { get; private set; }

    public Guid TrainerUserId { get; private set; }

    public string Title { get; private set; }

    public string? Goal { get; private set; }

    public DateOnly? EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }
}
