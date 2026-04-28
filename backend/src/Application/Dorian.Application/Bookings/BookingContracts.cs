namespace Dorian.Application.Bookings;

using Dorian.Modules.Classes.Domain.Entities;

public sealed record BookingResponse(
    Guid Id,
    Guid CustomerId,
    Guid ClassSessionId,
    Guid BranchId,
    BookingStatus Status,
    DateTimeOffset BookedAt,
    DateTimeOffset? CancelledAt);

public sealed record CreateBookingRequest(Guid CustomerId);
