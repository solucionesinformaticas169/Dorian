namespace Dorian.Application.Dashboard;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Classes.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class DashboardService : IDashboardService
{
    private static readonly TimeSpan EcuadorOffset = TimeSpan.FromHours(-5);
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        var scopeBranchId = ResolveBranchScope(user);
        var now = DateTimeOffset.UtcNow;
        var (todayStartUtc, tomorrowStartUtc) = ResolveTodayWindowUtc(now);

        var branchesQuery = _dbContext.Branches.AsNoTracking().Where(x => x.IsActive);
        if (scopeBranchId.HasValue)
        {
            branchesQuery = branchesQuery.Where(x => x.Id == scopeBranchId.Value);
        }

        var branches = await branchesQuery
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        var branchIds = branches.Select(x => x.Id).ToArray();
        if (branchIds.Length == 0)
        {
            return new DashboardSummaryResponse(
                0,
                0,
                0,
                0m,
                "Sin actividad",
                [],
                [],
                "Suma del precio de planes activos asignados a clientes activos.");
        }

        var activeCustomers = await _dbContext.Customers
            .AsNoTracking()
            .Where(x => x.Status == CustomerStatus.Active)
            .Select(x => new
            {
                x.Id,
                x.ActiveMembershipId,
                x.ActiveMembershipStartsAtUtc,
                x.ActiveMembershipEndsAtUtc
            })
            .ToListAsync(cancellationToken);

        var todayClasses = await _dbContext.ClassSessions
            .AsNoTracking()
            .Where(x =>
                branchIds.Contains(x.BranchId)
                && x.Status == ClassSessionStatus.Scheduled
                && x.StartTime >= todayStartUtc
                && x.StartTime < tomorrowStartUtc)
            .Select(x => new
            {
                x.Id,
                x.BranchId,
                x.Name,
                x.StartTime,
                x.Capacity
            })
            .ToListAsync(cancellationToken);

        var classIds = todayClasses.Select(x => x.Id).ToArray();
        var reservedByClass = classIds.Length == 0
            ? new Dictionary<Guid, int>()
            : await _dbContext.Bookings
                .AsNoTracking()
                .Where(x => classIds.Contains(x.ClassSessionId) && x.Status == BookingStatus.Reserved)
                .GroupBy(x => x.ClassSessionId)
                .Select(group => new { ClassSessionId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.ClassSessionId, x => x.Count, cancellationToken);

        var todayCheckIns = await _dbContext.CheckIns
            .AsNoTracking()
            .Where(x =>
                branchIds.Contains(x.BranchId)
                && x.Status == CheckInStatus.Accepted
                && x.CheckedInAt >= todayStartUtc
                && x.CheckedInAt < tomorrowStartUtc)
            .GroupBy(x => x.BranchId)
            .Select(group => new { BranchId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(x => x.BranchId, x => x.Count, cancellationToken);

        var createdClassesByBranch = await _dbContext.ClassSessions
            .AsNoTracking()
            .Where(x => branchIds.Contains(x.BranchId))
            .GroupBy(x => x.BranchId)
            .Select(group => new { BranchId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(x => x.BranchId, x => x.Count, cancellationToken);

        var membershipIds = activeCustomers
            .Where(x =>
                x.ActiveMembershipId.HasValue
                && x.ActiveMembershipStartsAtUtc.HasValue
                && x.ActiveMembershipEndsAtUtc.HasValue
                && x.ActiveMembershipStartsAtUtc.Value <= now
                && x.ActiveMembershipEndsAtUtc.Value >= now)
            .Select(x => x.ActiveMembershipId!.Value)
            .Distinct()
            .ToArray();

        var membershipsById = membershipIds.Length == 0
            ? new Dictionary<Guid, decimal>()
            : await _dbContext.Memberships
                .AsNoTracking()
                .Where(x => membershipIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Price, cancellationToken);

        var estimatedRevenue = activeCustomers.Sum(customer =>
        {
            if (!customer.ActiveMembershipId.HasValue) return 0m;
            if (!customer.ActiveMembershipStartsAtUtc.HasValue || !customer.ActiveMembershipEndsAtUtc.HasValue) return 0m;
            if (customer.ActiveMembershipStartsAtUtc.Value > now || customer.ActiveMembershipEndsAtUtc.Value < now) return 0m;
            return membershipsById.GetValueOrDefault(customer.ActiveMembershipId.Value);
        });

        var branchActivity = branches
            .Select(branch =>
            {
                var branchCreatedClasses = createdClassesByBranch.GetValueOrDefault(branch.Id);
                var branchClasses = todayClasses.Count(classSession => classSession.BranchId == branch.Id);
                var branchCheckIns = todayCheckIns.GetValueOrDefault(branch.Id);
                return new BranchActivityPoint(
                    branch.Id,
                    branch.Name,
                    branchCreatedClasses,
                    branchClasses,
                    branchCheckIns);
            })
            .OrderByDescending(x => x.CreatedClassesCount)
            .ThenByDescending(x => x.TodayCheckInsCount)
            .ThenBy(x => x.BranchName)
            .ToArray();

        var branchNameById = branches.ToDictionary(x => x.Id, x => x.Name);
        var classOccupancy = todayClasses
            .Select(classSession =>
            {
                var reservedSpots = reservedByClass.GetValueOrDefault(classSession.Id);
                var occupancyRate = classSession.Capacity <= 0
                    ? 0m
                    : Math.Round((decimal)reservedSpots / classSession.Capacity * 100m, 2);

                return new ClassOccupancyPoint(
                    classSession.Id,
                    classSession.Name,
                    branchNameById[classSession.BranchId],
                    classSession.StartTime,
                    reservedSpots,
                    classSession.Capacity,
                    occupancyRate);
            })
            .OrderByDescending(x => x.OccupancyRate)
            .ThenBy(x => x.StartTime)
            .ToArray();

        return new DashboardSummaryResponse(
            activeCustomers.Count,
            todayClasses.Count,
            todayCheckIns.Values.Sum(),
            estimatedRevenue,
            branchActivity.FirstOrDefault()?.BranchName ?? "Sin actividad",
            branchActivity,
            classOccupancy,
            "Suma del precio de planes activos asignados a clientes activos.");
    }

    private static (DateTimeOffset TodayStartUtc, DateTimeOffset TomorrowStartUtc) ResolveTodayWindowUtc(DateTimeOffset nowUtc)
    {
        var localNow = nowUtc.ToOffset(EcuadorOffset);
        var localToday = new DateTimeOffset(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0, EcuadorOffset);
        return (localToday.ToUniversalTime(), localToday.AddDays(1).ToUniversalTime());
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static Guid? ResolveBranchScope(CurrentUser user)
    {
        if (user.IsInRole(RoleNames.SuperAdmin)) return null;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId.HasValue) return user.BranchId.Value;
        throw new ForbiddenException("You do not have access to dashboard metrics.");
    }
}
