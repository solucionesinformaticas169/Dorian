namespace Dorian.Modules.Classes.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Booking : AuditableEntity<Guid>
{
    private Booking() : base(Guid.Empty)
    {
    }

    public Booking(Guid id, Guid customerId, Guid classSessionId) : base(id)
    {
        CustomerId = customerId;
        ClassSessionId = classSessionId;
        Status = BookingStatus.Reserved;
        BookedAt = DateTimeOffset.UtcNow;
    }

    public Guid CustomerId { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTimeOffset BookedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
    }

    public void MarkAttended()
    {
        Status = BookingStatus.Attended;
    }
}
