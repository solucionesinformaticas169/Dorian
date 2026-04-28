namespace Dorian.Modules.Auditing.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class AuditLog : Entity<Guid>
{
    private AuditLog() : base(Guid.Empty)
    {
    }

    public AuditLog(Guid id, Guid? actorUserId, Guid? branchId, string action, string entityName, string entityId, string? metadataJson) : base(id)
    {
        ActorUserId = actorUserId;
        BranchId = branchId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        MetadataJson = metadataJson;
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid? ActorUserId { get; private set; }

    public Guid? BranchId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string EntityName { get; private set; } = string.Empty;

    public string EntityId { get; private set; } = string.Empty;

    public string? MetadataJson { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }
}
