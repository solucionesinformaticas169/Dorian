namespace Dorian.Application.Bookings;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Classes.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class BookingService : IBookingService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public BookingService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<BookingResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        IQueryable<Booking> query = _dbContext.Bookings.AsNoTracking();

        if (user.IsInRole(RoleNames.SuperAdmin))
            return await Project(query).ToListAsync(cancellationToken);

        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId.HasValue)
            return await Project(query.Where(x => _dbContext.Customers.Any(c => c.Id == x.CustomerId && c.BranchId == user.BranchId.Value))).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.Trainer) && user.UserId.HasValue)
            return await Project(query.Where(x => _dbContext.ClassSessions.Any(c => c.Id == x.ClassSessionId && c.TrainerUserId == user.UserId.Value))).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.Customer) && user.UserId.HasValue)
            return await Project(query.Where(x => _dbContext.Customers.Any(c => c.Id == x.CustomerId && c.UserId == user.UserId.Value))).ToListAsync(cancellationToken);

        throw new ForbiddenException("You do not have access to bookings.");
    }

    public async Task<IReadOnlyCollection<BookingResponse>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken) ?? throw new NotFoundException("Customer not found.");
        EnsureCanViewCustomer(customer.UserId, customer.BranchId);
        return await Project(_dbContext.Bookings.AsNoTracking().Where(x => x.CustomerId == customerId)).ToListAsync(cancellationToken);
    }

    public async Task<BookingResponse> CreateAsync(Guid classSessionId, CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var currentUser = EnsureAuthenticated();
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken) ?? throw new NotFoundException("Customer not found.");
        var classSession = await _dbContext.ClassSessions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == classSessionId, cancellationToken) ?? throw new NotFoundException("Class not found.");

        EnsureCanCreateBooking(currentUser, customer.UserId, classSession.BranchId);

        if (classSession.Status != ClassSessionStatus.Scheduled)
            throw new BookingValidationException("Only scheduled classes can be booked.");

        if (await _dbContext.Bookings.AnyAsync(x => x.CustomerId == request.CustomerId && x.ClassSessionId == classSessionId && x.Status != BookingStatus.Cancelled, cancellationToken))
            throw new BookingValidationException("The customer already has a booking for this class.");

        var reservedSpots = await _dbContext.Bookings.CountAsync(x => x.ClassSessionId == classSessionId && x.Status == BookingStatus.Reserved, cancellationToken);
        if (reservedSpots >= classSession.Capacity)
            throw new BookingValidationException("The class is full.");

        var booking = new Booking(Guid.NewGuid(), request.CustomerId, classSessionId);
        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Project(_dbContext.Bookings.AsNoTracking().Where(x => x.Id == booking.Id)).SingleAsync(cancellationToken);
    }

    public async Task<BookingResponse> CancelAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.Bookings.SingleOrDefaultAsync(x => x.Id == bookingId, cancellationToken) ?? throw new NotFoundException("Booking not found.");
        var customer = await _dbContext.Customers.AsNoTracking().SingleAsync(x => x.Id == booking.CustomerId, cancellationToken);
        EnsureCanCancelBooking(customer.UserId, customer.BranchId);

        if (booking.Status != BookingStatus.Reserved)
            throw new BookingValidationException("Only reserved bookings can be cancelled.");

        booking.Cancel();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Project(_dbContext.Bookings.AsNoTracking().Where(x => x.Id == booking.Id)).SingleAsync(cancellationToken);
    }

    public async Task<BookingResponse> AttendAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.Bookings.SingleOrDefaultAsync(x => x.Id == bookingId, cancellationToken) ?? throw new NotFoundException("Booking not found.");
        var customer = await _dbContext.Customers.AsNoTracking().SingleAsync(x => x.Id == booking.CustomerId, cancellationToken);
        EnsureCanMarkAttendance(customer.BranchId);

        if (booking.Status != BookingStatus.Reserved)
            throw new BookingValidationException("Only reserved bookings can be marked as attended.");

        booking.MarkAttended();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Project(_dbContext.Bookings.AsNoTracking().Where(x => x.Id == booking.Id)).SingleAsync(cancellationToken);
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private void EnsureCanViewCustomer(Guid customerUserId, Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == branchId) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customerUserId) return;
        throw new ForbiddenException("You cannot view this customer's bookings.");
    }

    private void EnsureCanCreateBooking(CurrentUser currentUser, Guid customerUserId, Guid classBranchId)
    {
        if (currentUser.IsInRole(RoleNames.SuperAdmin)) return;
        if (currentUser.IsInRole(RoleNames.BranchAdmin) && currentUser.BranchId == classBranchId) return;
        if (currentUser.IsInRole(RoleNames.Customer) && currentUser.UserId == customerUserId) return;
        throw new ForbiddenException("You cannot create this booking.");
    }

    private void EnsureCanCancelBooking(Guid customerUserId, Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId == branchId) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customerUserId) return;
        throw new ForbiddenException("You cannot cancel this booking.");
    }

    private void EnsureCanMarkAttendance(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot mark attendance for this booking.");
    }

    private IQueryable<BookingResponse> Project(IQueryable<Booking> query)
    {
        return query
            .OrderByDescending(x => x.BookedAt)
            .Select(x => new BookingResponse(
                x.Id,
                x.CustomerId,
                x.ClassSessionId,
                _dbContext.ClassSessions.Where(c => c.Id == x.ClassSessionId).Select(c => c.BranchId).First(),
                x.Status,
                x.BookedAt,
                x.CancelledAt));
    }

    private sealed class BookingValidationException : AppException
    {
        public BookingValidationException(string message) : base(message) { }
    }
}
