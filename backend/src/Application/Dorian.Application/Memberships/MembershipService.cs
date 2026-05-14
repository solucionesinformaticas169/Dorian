namespace Dorian.Application.Memberships;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Memberships.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class MembershipService : IMembershipService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public MembershipService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<MembershipResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsurePlanReader();
        IQueryable<Membership> query = _dbContext.Memberships.AsNoTracking();
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception) || user.IsInRole(RoleNames.Trainer)) && user.BranchId.HasValue)
            query = query.Where(x => x.BranchId == null || x.BranchId == user.BranchId.Value);

        return await query.OrderBy(x => x.Name).Select(MapExpression).ToListAsync(cancellationToken);
    }

    public async Task<MembershipResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.Memberships.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Membership not found.");
        EnsureCanReadBranch(membership.BranchId);
        return Map(membership);
    }

    public async Task<MembershipResponse> CreateAsync(CreateMembershipRequest request, CancellationToken cancellationToken)
    {
        EnsureCanManageBranch(request.BranchId);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        var membership = new Membership(Guid.NewGuid(), request.BranchId, request.Name, request.DurationInDays, request.Price, request.Currency, request.IsActive);
        _dbContext.Memberships.Add(membership);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(membership);
    }

    public async Task<MembershipResponse> UpdateAsync(Guid id, UpdateMembershipRequest request, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.Memberships.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Membership not found.");
        EnsureCanManageBranch(membership.BranchId);
        EnsureCanManageBranch(request.BranchId);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        membership.Update(request.BranchId, request.Name, request.DurationInDays, request.Price, request.Currency, request.IsActive);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(membership);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.Memberships.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Membership not found.");
        EnsureCanManageBranch(membership.BranchId);
        _dbContext.Memberships.Remove(membership);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private CurrentUser EnsurePlanReader()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        if (user.IsInRole(RoleNames.SuperAdmin) || user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception) || user.IsInRole(RoleNames.Trainer)) return user;
        throw new ForbiddenException("You do not have access to plans.");
    }

    private void EnsureCanManageBranch(Guid? branchId)
    {
        var user = EnsurePlanReader();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && branchId.HasValue && user.BranchId == branchId.Value) return;
        throw new ForbiddenException("You cannot manage plans for this branch.");
    }

    private void EnsureCanReadBranch(Guid? branchId)
    {
        var user = EnsurePlanReader();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception) || user.IsInRole(RoleNames.Trainer)) && (!branchId.HasValue || user.BranchId == branchId.Value)) return;
        throw new ForbiddenException("You cannot view plans for this branch.");
    }

    private async Task EnsureBranchExists(Guid? branchId, CancellationToken cancellationToken)
    {
        if (!branchId.HasValue)
            return;

        if (!await _dbContext.Branches.AnyAsync(x => x.Id == branchId.Value, cancellationToken))
            throw new NotFoundException("Branch not found.");
    }

    private static readonly System.Linq.Expressions.Expression<Func<Membership, MembershipResponse>> MapExpression = membership => new MembershipResponse(membership.Id, membership.BranchId, membership.Name, membership.DurationInDays, membership.Price, membership.Currency, membership.IsActive, membership.CreatedAtUtc, membership.UpdatedAtUtc);
    private static MembershipResponse Map(Membership membership) => new(membership.Id, membership.BranchId, membership.Name, membership.DurationInDays, membership.Price, membership.Currency, membership.IsActive, membership.CreatedAtUtc, membership.UpdatedAtUtc);
}
