namespace Dorian.Application.Promotions;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Promotions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class PromotionService : IPromotionService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public PromotionService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<PromotionResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        IQueryable<Promotion> query = _dbContext.Promotions.AsNoTracking();

        if (user.IsInRole(RoleNames.SuperAdmin))
            return await query.OrderByDescending(x => x.StartsAt).Select(MapExpression).ToListAsync(cancellationToken);

        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId.HasValue)
            return await query.Where(x => x.BranchId == null || x.BranchId == user.BranchId.Value).OrderByDescending(x => x.StartsAt).Select(MapExpression).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.Customer))
            return await query.Where(IsVisibleForCustomers()).OrderByDescending(x => x.StartsAt).Select(MapExpression).ToListAsync(cancellationToken);

        throw new ForbiddenException("You do not have access to promotions.");
    }

    public async Task<IReadOnlyCollection<PromotionResponse>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.Customer))
            return await _dbContext.Promotions.AsNoTracking().Where(x => (x.BranchId == null || x.BranchId == branchId) && x.Status == PromotionStatus.Active && x.StartsAt <= DateTimeOffset.UtcNow && x.EndsAt >= DateTimeOffset.UtcNow).OrderByDescending(x => x.StartsAt).Select(MapExpression).ToListAsync(cancellationToken);

        EnsureCanViewBranch(branchId);
        return await _dbContext.Promotions.AsNoTracking().Where(x => x.BranchId == null || x.BranchId == branchId).OrderByDescending(x => x.StartsAt).Select(MapExpression).ToListAsync(cancellationToken);
    }

    public async Task<PromotionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await _dbContext.Promotions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Promotion not found.");
        EnsureCanViewPromotion(promotion);
        return Map(promotion);
    }

    public async Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        EnsureCanManagePromotionBranch(request.BranchId, allowGlobalForBranchAdmin: false);
        await EnsureBranchExistsIfNeeded(request.BranchId, cancellationToken);

        var promotion = new Promotion(Guid.NewGuid(), request.BranchId, request.Title, request.Description, request.ImageUrl, request.DiscountType, request.DiscountValue, request.StartsAt, request.EndsAt, request.Status);
        _dbContext.Promotions.Add(promotion);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(promotion);
    }

    public async Task<PromotionResponse> UpdateAsync(Guid id, UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        var promotion = await _dbContext.Promotions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Promotion not found.");
        EnsureCanManagePromotionBranch(promotion.BranchId, allowGlobalForBranchAdmin: true);
        EnsureCanManagePromotionBranch(request.BranchId, allowGlobalForBranchAdmin: false);
        await EnsureBranchExistsIfNeeded(request.BranchId, cancellationToken);

        promotion.Update(request.BranchId, request.Title, request.Description, request.ImageUrl, request.DiscountType, request.DiscountValue, request.StartsAt, request.EndsAt, request.Status);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(promotion);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await _dbContext.Promotions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Promotion not found.");
        EnsureCanManagePromotionBranch(promotion.BranchId, allowGlobalForBranchAdmin: true);
        _dbContext.Promotions.Remove(promotion);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PromotionResponse> ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await _dbContext.Promotions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Promotion not found.");
        EnsureCanManagePromotionBranch(promotion.BranchId, allowGlobalForBranchAdmin: true);
        promotion.Activate();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(promotion);
    }

    public async Task<PromotionResponse> DisableAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await _dbContext.Promotions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Promotion not found.");
        EnsureCanManagePromotionBranch(promotion.BranchId, allowGlobalForBranchAdmin: true);
        promotion.Disable();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(promotion);
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private void EnsureCanViewBranch(Guid branchId)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot view promotions for this branch.");
    }

    private void EnsureCanViewPromotion(Promotion promotion)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && (promotion.BranchId is null || promotion.BranchId == user.BranchId)) return;
        if (user.IsInRole(RoleNames.Customer) && IsVisibleForCustomerEntity(promotion)) return;
        throw new ForbiddenException("You cannot view this promotion.");
    }

    private void EnsureCanManagePromotionBranch(Guid? branchId, bool allowGlobalForBranchAdmin)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin))
        {
            if (!branchId.HasValue)
            {
                if (allowGlobalForBranchAdmin)
                    throw new ForbiddenException("BranchAdmin cannot manage global promotions.");
                throw new ForbiddenException("BranchAdmin cannot create global promotions.");
            }

            if (user.BranchId == branchId) return;
        }

        throw new ForbiddenException("You cannot manage promotions for this branch.");
    }

    private async Task EnsureBranchExistsIfNeeded(Guid? branchId, CancellationToken cancellationToken)
    {
        if (!branchId.HasValue) return;
        if (!await _dbContext.Branches.AnyAsync(x => x.Id == branchId.Value, cancellationToken))
            throw new NotFoundException("Branch not found.");
    }

    private static System.Linq.Expressions.Expression<Func<Promotion, bool>> IsVisibleForCustomers()
    {
        return promotion => promotion.Status == PromotionStatus.Active
            && promotion.StartsAt <= DateTimeOffset.UtcNow
            && promotion.EndsAt >= DateTimeOffset.UtcNow;
    }

    private static bool IsVisibleForCustomerEntity(Promotion promotion)
    {
        return promotion.Status == PromotionStatus.Active
            && promotion.StartsAt <= DateTimeOffset.UtcNow
            && promotion.EndsAt >= DateTimeOffset.UtcNow;
    }

    private static readonly System.Linq.Expressions.Expression<Func<Promotion, PromotionResponse>> MapExpression = promotion => new PromotionResponse(promotion.Id, promotion.BranchId, promotion.Title, promotion.Description, promotion.ImageUrl, promotion.DiscountType, promotion.DiscountValue, promotion.StartsAt, promotion.EndsAt, promotion.Status, promotion.CreatedAtUtc, promotion.UpdatedAtUtc);
    private static PromotionResponse Map(Promotion promotion) => new(promotion.Id, promotion.BranchId, promotion.Title, promotion.Description, promotion.ImageUrl, promotion.DiscountType, promotion.DiscountValue, promotion.StartsAt, promotion.EndsAt, promotion.Status, promotion.CreatedAtUtc, promotion.UpdatedAtUtc);
}
