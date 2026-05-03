namespace Dorian.Application.Access;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class AccessService : IAccessService
{
    private static readonly TimeSpan DefaultAccessPassLifetime = TimeSpan.FromHours(12);

    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AccessService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<AccessPassResponse> GetAccessPassAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerWithUserAsync(customerId, cancellationToken);
        EnsureCanManageAccessPass(customer);

        var accessPass = await _dbContext.AccessPasses.SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        if (accessPass is null)
        {
            accessPass = new AccessPass(Guid.NewGuid(), customerId, GenerateQrCodeValue(), DateTimeOffset.UtcNow.Add(DefaultAccessPassLifetime));
            _dbContext.AccessPasses.Add(accessPass);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (accessPass.Status == AccessPassStatus.Active && accessPass.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            accessPass.MarkExpired();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Map(accessPass);
    }

    public async Task<AccessPassResponse> RegenerateAccessPassAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerWithUserAsync(customerId, cancellationToken);
        EnsureCanManageAccessPass(customer);

        var accessPass = await _dbContext.AccessPasses.SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        if (accessPass is null)
        {
            accessPass = new AccessPass(Guid.NewGuid(), customerId, GenerateQrCodeValue(), DateTimeOffset.UtcNow.Add(DefaultAccessPassLifetime));
            _dbContext.AccessPasses.Add(accessPass);
        }
        else
        {
            accessPass.Regenerate(GenerateQrCodeValue(), DateTimeOffset.UtcNow.Add(DefaultAccessPassLifetime));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(accessPass);
    }

    public async Task<CheckInResponse> ScanAsync(Guid branchId, ScanCheckInRequest request, CancellationToken cancellationToken)
    {
        EnsureCanProcessBranchCheckIn(branchId);

        var accessPass = await _dbContext.AccessPasses.AsNoTracking().SingleOrDefaultAsync(x => x.QrCodeValue == request.QrCodeValue, cancellationToken)
            ?? throw new NotFoundException("Access pass not found.");

        return await CreateCheckInForCustomerAsync(branchId, accessPass.CustomerId, CheckInSource.QrScan, accessPass.Id, cancellationToken);
    }

    public async Task<CheckInResponse> ManualAsync(Guid branchId, ManualCheckInRequest request, CancellationToken cancellationToken)
    {
        EnsureCanProcessBranchCheckIn(branchId);
        await EnsureCustomerExistsAsync(request.CustomerId, cancellationToken);
        return await CreateCheckInForCustomerAsync(branchId, request.CustomerId, CheckInSource.Manual, null, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CheckInResponse>> GetBranchCheckInsAsync(Guid branchId, CancellationToken cancellationToken)
    {
        EnsureCanViewBranchCheckIns(branchId);
        return await Project(_dbContext.CheckIns.AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .OrderByDescending(x => x.CheckedInAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CheckInResponse>> GetCustomerCheckInsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerWithUserAsync(customerId, cancellationToken);
        EnsureCanViewCustomerCheckIns(customer);
        return await Project(_dbContext.CheckIns.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CheckedInAt))
            .ToListAsync(cancellationToken);
    }

    private async Task<CheckInResponse> CreateCheckInForCustomerAsync(Guid branchId, Guid customerId, CheckInSource source, Guid? accessPassId, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.Include(x => x.User).SingleAsync(x => x.Id == customerId, cancellationToken);
        var accessPass = accessPassId.HasValue ? await _dbContext.AccessPasses.SingleAsync(x => x.Id == accessPassId.Value, cancellationToken) : await _dbContext.AccessPasses.SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        var rejectionReason = await ValidateAccessAsync(branchId, customer, accessPass, cancellationToken);
        if (accessPass is not null && accessPass.Status == AccessPassStatus.Active && accessPass.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            accessPass.MarkExpired();
        }

        var checkIn = CreateCheckIn(customer.Id, branchId, source, rejectionReason is null ? CheckInStatus.Accepted : CheckInStatus.Rejected, rejectionReason);
        _dbContext.CheckIns.Add(checkIn);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(checkIn);
    }

    private async Task<string?> ValidateAccessAsync(Guid branchId, Customer customer, AccessPass? accessPass, CancellationToken cancellationToken)
    {
        if (customer.BranchId != branchId) return "Customer does not belong to this branch.";
        if (customer.Status != CustomerStatus.Active) return "Customer is not active.";
        if (!customer.ActiveMembershipId.HasValue) return "Customer does not have an active membership assigned.";
        if (!customer.HasActiveMembership(DateTimeOffset.UtcNow)) return "Customer membership is expired or not yet active.";

        var membershipIsActive = await _dbContext.Memberships.AnyAsync(x => x.Id == customer.ActiveMembershipId.Value && x.BranchId == branchId && x.IsActive, cancellationToken);
        if (!membershipIsActive) return "Customer membership is not available for this branch.";

        if (accessPass is null) return "Access pass not found.";
        if (accessPass.Status == AccessPassStatus.Revoked) return "Access pass has been revoked.";
        if (accessPass.ExpiresAt <= DateTimeOffset.UtcNow || accessPass.Status == AccessPassStatus.Expired) return "Access pass is expired.";

        return null;
    }

    private IQueryable<CheckInResponse> Project(IQueryable<CheckIn> query)
    {
        return query.Select(x => new CheckInResponse(
            x.Id,
            x.CustomerId,
            x.BranchId,
            x.CheckedInAt,
            x.CheckedInByUserId,
            x.Source,
            x.Status,
            x.RejectionReason,
            x.CreatedAtUtc,
            x.UpdatedAtUtc));
    }

    private Customer EnsureCustomerRole(Customer customer)
    {
        return customer;
    }

    private async Task<Customer> GetCustomerWithUserAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Customers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");
    }

    private async Task EnsureCustomerExistsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Customers.AnyAsync(x => x.Id == customerId, cancellationToken))
            throw new NotFoundException("Customer not found.");
    }

    private void EnsureCanManageAccessPass(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        throw new ForbiddenException("You cannot access this customer's QR pass.");
    }

    private void EnsureCanProcessBranchCheckIn(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.Reception) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot process check-ins for this branch.");
    }

    private void EnsureCanViewBranchCheckIns(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot view check-ins for this branch.");
    }

    private void EnsureCanViewCustomerCheckIns(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == customer.BranchId) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        throw new ForbiddenException("You cannot view this customer's check-ins.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private CheckIn CreateCheckIn(Guid customerId, Guid branchId, CheckInSource source, CheckInStatus status, string? rejectionReason)
    {
        return new CheckIn(Guid.NewGuid(), customerId, branchId, DateTimeOffset.UtcNow, _currentUserService.User.UserId, source, status, rejectionReason);
    }

    private static AccessPassResponse Map(AccessPass accessPass) => new(accessPass.Id, accessPass.CustomerId, accessPass.QrCodeValue, accessPass.ExpiresAt, accessPass.Status, accessPass.CreatedAtUtc, accessPass.UpdatedAtUtc);
    private static CheckInResponse Map(CheckIn checkIn) => new(checkIn.Id, checkIn.CustomerId, checkIn.BranchId, checkIn.CheckedInAt, checkIn.CheckedInByUserId, checkIn.Source, checkIn.Status, checkIn.RejectionReason, checkIn.CreatedAtUtc, checkIn.UpdatedAtUtc);
    private static string GenerateQrCodeValue() => $"ACC-{Guid.NewGuid():N}";
}

