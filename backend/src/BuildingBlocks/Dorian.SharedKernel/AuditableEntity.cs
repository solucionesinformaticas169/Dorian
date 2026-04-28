namespace Dorian.SharedKernel.Primitives;

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    protected AuditableEntity(TId id) : base(id)
    {
    }

    public DateTimeOffset CreatedAtUtc { get; protected set; } = DateTimeOffset.UtcNow;

    public Guid? CreatedByUserId { get; protected set; }

    public DateTimeOffset? UpdatedAtUtc { get; protected set; }

    public Guid? UpdatedByUserId { get; protected set; }

    protected void Touch(Guid? updatedByUserId)
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }
}
