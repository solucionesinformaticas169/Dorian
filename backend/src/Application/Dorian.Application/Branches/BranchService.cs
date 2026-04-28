namespace Dorian.Application.Branches;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class BranchService : IBranchService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public BranchService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<BranchResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        IQueryable<Branch> query = _dbContext.Branches.AsNoTracking();

        if (user.IsInRole(RoleNames.SuperAdmin))
            return await query.OrderBy(x => x.Name).Select(MapExpression).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId.HasValue)
            return await query.Where(x => x.Id == user.BranchId.Value).Select(MapExpression).ToListAsync(cancellationToken);

        throw new ForbiddenException("You do not have access to branches.");
    }

    public async Task<BranchResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var branch = await _dbContext.Branches.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Branch not found.");
        EnsureCanAccessBranch(branch.Id);
        return Map(branch);
    }

    public async Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.SuperAdmin))
            throw new ForbiddenException("Only SuperAdmin can create branches.");

        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        if (await _dbContext.Branches.AnyAsync(x => x.Code == normalizedCode, cancellationToken))
            throw new BranchValidationException("A branch with that code already exists.");

        var branch = new Branch(Guid.NewGuid(), request.Code, request.Name, request.City, request.Address, request.PhoneNumber);
        _dbContext.Branches.Add(branch);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(branch);
    }

    public async Task<BranchResponse> UpdateAsync(Guid id, UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        var branch = await _dbContext.Branches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Branch not found.");
        EnsureCanUpdateBranch(branch.Id);

        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        if (await _dbContext.Branches.AnyAsync(x => x.Id != id && x.Code == normalizedCode, cancellationToken))
            throw new BranchValidationException("A branch with that code already exists.");

        branch.Update(request.Code, request.Name, request.City, request.Address, request.PhoneNumber, request.IsActive);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(branch);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.SuperAdmin))
            throw new ForbiddenException("Only SuperAdmin can delete branches.");

        var branch = await _dbContext.Branches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Branch not found.");
        _dbContext.Branches.Remove(branch);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private void EnsureCanAccessBranch(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot access this branch.");
    }

    private void EnsureCanUpdateBranch(Guid branchId) => EnsureCanAccessBranch(branchId);

    private static readonly System.Linq.Expressions.Expression<Func<Branch, BranchResponse>> MapExpression = branch => new BranchResponse(branch.Id, branch.Code, branch.Name, branch.City, branch.Address, branch.PhoneNumber, branch.IsActive, branch.CreatedAtUtc, branch.UpdatedAtUtc);
    private static BranchResponse Map(Branch branch) => new(branch.Id, branch.Code, branch.Name, branch.City, branch.Address, branch.PhoneNumber, branch.IsActive, branch.CreatedAtUtc, branch.UpdatedAtUtc);

    private sealed class BranchValidationException : AppException
    {
        public BranchValidationException(string message) : base(message) { }
    }
}
