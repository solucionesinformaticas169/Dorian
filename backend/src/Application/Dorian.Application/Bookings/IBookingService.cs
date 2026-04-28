namespace Dorian.Application.Bookings;

public interface IBookingService
{
    Task<IReadOnlyCollection<BookingResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<BookingResponse>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    Task<BookingResponse> CreateAsync(Guid classSessionId, CreateBookingRequest request, CancellationToken cancellationToken);
    Task<BookingResponse> CancelAsync(Guid bookingId, CancellationToken cancellationToken);
    Task<BookingResponse> AttendAsync(Guid bookingId, CancellationToken cancellationToken);
}
