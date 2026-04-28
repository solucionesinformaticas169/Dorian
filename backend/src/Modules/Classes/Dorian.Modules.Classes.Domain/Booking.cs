namespace Dorian.Modules.Classes.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Booking : AuditableEntity<Guid>
{
    public Booking(Guid id, Guid classSessionId, Guid clientUserId) : base(id)
    {
        ClassSessionId = classSessionId;
        ClientUserId = clientUserId;
        Status = BookingStatus.Confirmed;
    }

    public Guid ClassSessionId { get; private set; }

    public Guid ClientUserId { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTimeOffset BookedAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }
}
