namespace Dorian.Modules.Auditing.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class AuditLog : Entity<Guid>
{
    public AuditLog(Guid id, string action, string entityName, string entityId) : base(id)
    {
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid? ActorUserId { get; private set; }

    public Guid? BranchId { get; private set; }

    public string Action { get; private set; }

    public string EntityName { get; private set; }

    public string EntityId { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }
}
