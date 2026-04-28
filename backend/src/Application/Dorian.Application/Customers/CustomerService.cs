namespace Dorian.Application.Customers;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class CustomerService : ICustomerService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppPasswordHasher _passwordHasher;

    public CustomerService(IDorianDbContext dbContext, ICurrentUserService currentUserService, IAppPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        IQueryable<Customer> query = QueryCustomers();

        if (user.IsInRole(RoleNames.SuperAdmin))
        {
            return await query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Select(MapExpression).ToListAsync(cancellationToken);
        }

        if (CanManageBranchCustomers(user) && user.BranchId.HasValue)
        {
            return await query.Where(x => x.BranchId == user.BranchId.Value).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Select(MapExpression).ToListAsync(cancellationToken);
        }

        throw new ForbiddenException("You do not have access to customers.");
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken)
    {
        EnsureCanViewBranch(branchId);
        return await QueryCustomers()
            .Where(x => x.BranchId == branchId)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(MapExpression)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await QueryCustomers().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Customer not found.");
        EnsureCanViewCustomer(customer);
        return Map(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.SuperAdmin) && !user.IsInRole(RoleNames.BranchAdmin) && !user.IsInRole(RoleNames.Reception))
        {
            throw new ForbiddenException("You cannot create customers.");
        }

        EnsureCanCreateForBranch(request.BranchId);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        await EnsureMembershipValid(request.BranchId, request.ActiveMembershipId, new CreateOrUpdateMembershipWindow(request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc), cancellationToken);
        await EnsureUniqueIdentity(request.IdentificationNumber, null, cancellationToken);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken))
        {
            throw new CustomerValidationException("A user with that email already exists.");
        }

        var customerRoleId = await _dbContext.Roles.Where(x => x.Name == RoleNames.Customer).Select(x => x.Id).SingleAsync(cancellationToken);
        var newUser = new User(Guid.NewGuid(), normalizedEmail, $"{request.FirstName} {request.LastName}", _passwordHasher.Hash(request.Password));
        newUser.UpdateProfile($"{request.FirstName} {request.LastName}", request.Phone);
        newUser.AssignToBranch(request.BranchId);
        newUser.SetRoles([customerRoleId]);
        newUser.SetActive(request.Status == CustomerStatus.Active);

        var customer = new Customer(Guid.NewGuid(), newUser.Id, request.BranchId, request.FirstName, request.LastName, request.IdentificationNumber, request.Phone, request.BirthDate, request.Gender, request.EmergencyContactName, request.EmergencyContactPhone, request.ActiveMembershipId, request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc, request.Status);

        _dbContext.Users.Add(newUser);
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        customer = await QueryCustomers().SingleAsync(x => x.Id == customer.Id, cancellationToken);
        return Map(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Customer not found.");
        EnsureCanManageCustomer(customer);
        EnsureCanCreateForBranch(request.BranchId);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        await EnsureMembershipValid(request.BranchId, request.ActiveMembershipId, new CreateOrUpdateMembershipWindow(request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc), cancellationToken);
        await EnsureUniqueIdentity(request.IdentificationNumber, customer.Id, cancellationToken);

        customer.Update(request.BranchId, request.FirstName, request.LastName, request.IdentificationNumber, request.Phone, request.BirthDate, request.Gender, request.EmergencyContactName, request.EmergencyContactPhone, request.ActiveMembershipId, request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc, request.Status);
        customer.User.UpdateProfile($"{request.FirstName} {request.LastName}", request.Phone);
        customer.User.AssignToBranch(request.BranchId);
        customer.User.SetActive(request.Status == CustomerStatus.Active);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updated = await QueryCustomers().SingleAsync(x => x.Id == id, cancellationToken);
        return Map(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Customer not found.");
        EnsureCanManageCustomer(customer);

        _dbContext.Customers.Remove(customer);
        _dbContext.Users.Remove(customer.User);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Customer> QueryCustomers() => _dbContext.Customers.AsNoTracking().Include(x => x.User);

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static bool CanManageBranchCustomers(CurrentUser user) => user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception);

    private void EnsureCanViewBranch(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (CanManageBranchCustomers(user) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot view customers for this branch.");
    }

    private void EnsureCanCreateForBranch(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (CanManageBranchCustomers(user) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot manage customers for this branch.");
    }

    private void EnsureCanManageCustomer(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId == customer.BranchId) return;
        throw new ForbiddenException("You cannot manage this customer.");
    }

    private void EnsureCanViewCustomer(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == customer.BranchId) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        throw new ForbiddenException("You cannot view this customer.");
    }

    private async Task EnsureBranchExists(Guid branchId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Branches.AnyAsync(x => x.Id == branchId, cancellationToken))
            throw new NotFoundException("Branch not found.");
    }

    private async Task EnsureMembershipValid(Guid branchId, Guid? membershipId, CreateOrUpdateMembershipWindow request, CancellationToken cancellationToken)
    {
        if (!membershipId.HasValue)
        {
            if (request.ActiveMembershipStartsAtUtc.HasValue || request.ActiveMembershipEndsAtUtc.HasValue)
                throw new CustomerValidationException("Membership dates require an active membership.");
            return;
        }
        if (!request.ActiveMembershipStartsAtUtc.HasValue || !request.ActiveMembershipEndsAtUtc.HasValue)
            throw new CustomerValidationException("Membership dates are required when an active membership is assigned.");

        var exists = await _dbContext.Memberships.AnyAsync(x => x.Id == membershipId.Value && x.BranchId == branchId && x.IsActive, cancellationToken);
        if (!exists) throw new NotFoundException("Membership not found for the selected branch.");
    }

    private async Task EnsureUniqueIdentity(string identificationNumber, Guid? customerId, CancellationToken cancellationToken)
    {
        var normalized = identificationNumber.Trim().ToUpperInvariant();
        var exists = await _dbContext.Customers.AnyAsync(x => x.IdentificationNumber == normalized && (!customerId.HasValue || x.Id != customerId.Value), cancellationToken);
        if (exists) throw new CustomerValidationException("A customer with that identification number already exists.");
    }

    private static readonly System.Linq.Expressions.Expression<Func<Customer, CustomerResponse>> MapExpression = customer => new CustomerResponse(customer.Id, customer.UserId, customer.User.Email, customer.BranchId, customer.ActiveMembershipId, customer.ActiveMembershipStartsAtUtc, customer.ActiveMembershipEndsAtUtc, customer.FirstName, customer.LastName, customer.IdentificationNumber, customer.Phone, customer.BirthDate, customer.Gender, customer.EmergencyContactName, customer.EmergencyContactPhone, customer.Status, customer.CreatedAtUtc, customer.UpdatedAtUtc);
    private static CustomerResponse Map(Customer customer) => new(customer.Id, customer.UserId, customer.User.Email, customer.BranchId, customer.ActiveMembershipId, customer.ActiveMembershipStartsAtUtc, customer.ActiveMembershipEndsAtUtc, customer.FirstName, customer.LastName, customer.IdentificationNumber, customer.Phone, customer.BirthDate, customer.Gender, customer.EmergencyContactName, customer.EmergencyContactPhone, customer.Status, customer.CreatedAtUtc, customer.UpdatedAtUtc);

    private sealed record CreateOrUpdateMembershipWindow(DateTimeOffset? ActiveMembershipStartsAtUtc, DateTimeOffset? ActiveMembershipEndsAtUtc);

    private sealed class CustomerValidationException : AppException
    {
        public CustomerValidationException(string message) : base(message) { }
    }
}

