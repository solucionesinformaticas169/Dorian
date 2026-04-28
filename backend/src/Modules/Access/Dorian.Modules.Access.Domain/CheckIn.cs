namespace Dorian.Modules.Access.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class CheckIn : AuditableEntity<Guid>
{
    private CheckIn() : base(Guid.Empty)
    {
    }

    public CheckIn(
        Guid id,
        Guid customerId,
        Guid branchId,
        DateTimeOffset checkedInAt,
        Guid? checkedInByUserId,
        CheckInSource source,
        CheckInStatus status,
        string? rejectionReason) : base(id)
    {
        CustomerId = customerId;
        BranchId = branchId;
        CheckedInAt = checkedInAt;
        CheckedInByUserId = checkedInByUserId;
        Source = source;
        Status = status;
        RejectionReason = string.IsNullOrWhiteSpace(rejectionReason) ? null : rejectionReason.Trim();
    }

    public Guid CustomerId { get; private set; }
    public Guid BranchId { get; private set; }
    public DateTimeOffset CheckedInAt { get; private set; }
    public Guid? CheckedInByUserId { get; private set; }
    public CheckInSource Source { get; private set; }
    public CheckInStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
}
