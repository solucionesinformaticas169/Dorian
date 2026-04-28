namespace Dorian.Modules.Classes.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class ClassSession : AuditableEntity<Guid>
{
    public ClassSession(Guid id, Guid branchId, Guid trainerUserId, string title, DateTimeOffset startsAtUtc, int durationMinutes, int capacity) : base(id)
    {
        BranchId = branchId;
        TrainerUserId = trainerUserId;
        Title = title;
        StartsAtUtc = startsAtUtc;
        DurationMinutes = durationMinutes;
        Capacity = capacity;
    }

    public Guid BranchId { get; private set; }

    public Guid TrainerUserId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public int DurationMinutes { get; private set; }

    public int Capacity { get; private set; }
}
