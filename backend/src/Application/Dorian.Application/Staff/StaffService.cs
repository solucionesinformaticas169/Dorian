namespace Dorian.Application.Staff;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class StaffService : IStaffService
{
    private static readonly string[] StaffRoles = [RoleNames.SuperAdmin, RoleNames.BranchAdmin, RoleNames.Reception, RoleNames.Trainer];
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppPasswordHasher _passwordHasher;

    public StaffService(IDorianDbContext dbContext, ICurrentUserService currentUserService, IAppPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyCollection<StaffMemberResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var currentUser = EnsureAuthenticated();
        var query = QueryStaff();

        if (currentUser.IsInRole(RoleNames.SuperAdmin))
        {
            return await ProjectAsync(query, cancellationToken);
        }

        if (currentUser.IsInRole(RoleNames.BranchAdmin) && currentUser.BranchId.HasValue)
        {
            return await ProjectAsync(query.Where(x => x.BranchId == currentUser.BranchId.Value), cancellationToken);
        }

        throw new ForbiddenException("You do not have access to staff management.");
    }

    public async Task<StaffMemberResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var staffMember = await QueryStaff().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Staff member not found.");
        EnsureCanView(staffMember);
        return await MapAsync(staffMember, cancellationToken);
    }

    public async Task<StaffMemberResponse> CreateAsync(CreateStaffMemberRequest request, CancellationToken cancellationToken)
    {
        var currentUser = EnsureAuthenticated();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken))
        {
            throw new StaffValidationException("A user with that email already exists.");
        }

        await EnsureBranchIsAllowedAsync(request.BranchId, request.Role, cancellationToken);
        EnsureCanManageRole(currentUser, request.Role, request.BranchId, existingBranchId: null);

        var roleId = await _dbContext.Roles.Where(x => x.Name == request.Role).Select(x => x.Id).SingleOrDefaultAsync(cancellationToken);
        if (roleId == Guid.Empty)
        {
            throw new NotFoundException("Role not found.");
        }

        var user = new User(Guid.NewGuid(), normalizedEmail, request.FullName, _passwordHasher.Hash(request.Password));
        user.UpdateProfile(request.FullName, request.PhoneNumber);
        user.AssignToBranch(request.BranchId);
        user.SetRoles([roleId]);
        user.SetActive(true);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(user.Id, cancellationToken);
    }

    public async Task<StaffMemberResponse> UpdateAsync(Guid id, UpdateStaffMemberRequest request, CancellationToken cancellationToken)
    {
        var currentUser = EnsureAuthenticated();
        var user = await _dbContext.Users.Include(x => x.UserRoles).SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Staff member not found.");
        var existingRole = await GetPrimaryRoleAsync(user, cancellationToken);
        if (string.IsNullOrWhiteSpace(existingRole))
        {
            throw new StaffValidationException("Staff member does not have a valid role assigned.");
        }

        EnsureCanView(user, existingRole);
        EnsureCanManageRole(currentUser, request.Role, request.BranchId, user.BranchId);
        await EnsureBranchIsAllowedAsync(request.BranchId, request.Role, cancellationToken);

        if (currentUser.UserId == user.Id && !request.IsActive)
        {
            throw new StaffValidationException("You cannot deactivate your own account.");
        }

        var roleId = await _dbContext.Roles.Where(x => x.Name == request.Role).Select(x => x.Id).SingleOrDefaultAsync(cancellationToken);
        if (roleId == Guid.Empty)
        {
            throw new NotFoundException("Role not found.");
        }

        user.UpdateProfile(request.FullName, request.PhoneNumber);
        user.AssignToBranch(request.BranchId);
        user.SetRoles([roleId]);
        user.SetActive(request.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.SetPasswordHash(_passwordHasher.Hash(request.Password));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(user.Id, cancellationToken);
    }

    public async Task DisableAsync(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = EnsureAuthenticated();
        var user = await _dbContext.Users.Include(x => x.UserRoles).SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Staff member not found.");
        var existingRole = await GetPrimaryRoleAsync(user, cancellationToken);
        if (string.IsNullOrWhiteSpace(existingRole))
        {
            throw new StaffValidationException("Staff member does not have a valid role assigned.");
        }

        EnsureCanView(user, existingRole);
        EnsureCanManageRole(currentUser, existingRole, user.BranchId, user.BranchId);

        if (currentUser.UserId == user.Id)
        {
            throw new StaffValidationException("You cannot deactivate your own account.");
        }

        user.Deactivate();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> QueryStaff()
        => _dbContext.Users
            .AsNoTracking()
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Where(x => x.UserRoles.Any(role => StaffRoles.Contains(role.Role.Name)));

    private async Task<IReadOnlyCollection<StaffMemberResponse>> ProjectAsync(IQueryable<User> query, CancellationToken cancellationToken)
    {
        var users = await query.OrderBy(x => x.FullName).ThenBy(x => x.Email).ToListAsync(cancellationToken);
        return await MapCollectionAsync(users, cancellationToken);
    }

    private async Task<IReadOnlyCollection<StaffMemberResponse>> MapCollectionAsync(IReadOnlyCollection<User> users, CancellationToken cancellationToken)
    {
        var branchIds = users.Where(x => x.BranchId.HasValue).Select(x => x.BranchId!.Value).Distinct().ToArray();
        var branchMap = await _dbContext.Branches.AsNoTracking()
            .Where(x => branchIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        return users.Select(user => Map(user, branchMap.TryGetValue(user.BranchId ?? Guid.Empty, out var branchName) ? branchName : null)).ToArray();
    }

    private async Task<StaffMemberResponse> MapAsync(User user, CancellationToken cancellationToken)
    {
        var branchName = user.BranchId.HasValue
            ? await _dbContext.Branches.AsNoTracking().Where(x => x.Id == user.BranchId.Value).Select(x => x.Name).SingleOrDefaultAsync(cancellationToken)
            : null;
        return Map(user, branchName);
    }

    private static StaffMemberResponse Map(User user, string? branchName)
    {
        var roles = user.UserRoles.Select(x => x.Role.Name).Distinct().OrderBy(RoleRank).ToArray();
        var primaryRole = roles.FirstOrDefault() ?? "Sin rol";
        return new StaffMemberResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.BranchId,
            branchName,
            roles,
            primaryRole,
            user.IsActive,
            user.CreatedAtUtc,
            user.UpdatedAtUtc);
    }

    private static int RoleRank(string role)
        => role switch
        {
            RoleNames.SuperAdmin => 0,
            RoleNames.BranchAdmin => 1,
            RoleNames.Reception => 2,
            RoleNames.Trainer => 3,
            _ => 9,
        };

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private void EnsureCanView(User staffMember, string? existingRole = null)
    {
        var currentUser = EnsureAuthenticated();
        var role = existingRole ?? staffMember.UserRoles.Select(x => x.Role.Name).OrderBy(RoleRank).FirstOrDefault();

        if (currentUser.IsInRole(RoleNames.SuperAdmin)) return;
        if (currentUser.IsInRole(RoleNames.BranchAdmin) && currentUser.BranchId.HasValue && staffMember.BranchId == currentUser.BranchId.Value && role is RoleNames.BranchAdmin or RoleNames.Reception or RoleNames.Trainer) return;
        throw new ForbiddenException("You do not have access to this staff member.");
    }

    private async Task EnsureBranchIsAllowedAsync(Guid? branchId, string role, CancellationToken cancellationToken)
    {
        if (role == RoleNames.SuperAdmin)
        {
            return;
        }

        if (!branchId.HasValue)
        {
            throw new StaffValidationException("This role must be linked to a branch.");
        }

        var exists = await _dbContext.Branches.AnyAsync(x => x.Id == branchId.Value, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Branch not found.");
        }
    }

    private void EnsureCanManageRole(CurrentUser currentUser, string desiredRole, Guid? desiredBranchId, Guid? existingBranchId)
    {
        if (desiredRole == RoleNames.SuperAdmin)
        {
            throw new ForbiddenException("SuperAdmin users are not managed from this module.");
        }

        if (currentUser.IsInRole(RoleNames.SuperAdmin))
        {
            return;
        }

        if (!currentUser.IsInRole(RoleNames.BranchAdmin) || !currentUser.BranchId.HasValue)
        {
            throw new ForbiddenException("You do not have access to manage staff.");
        }

        if (desiredRole == RoleNames.BranchAdmin)
        {
            throw new ForbiddenException("BranchAdmin users can only be created by SuperAdmin.");
        }

        var branchId = desiredBranchId ?? existingBranchId;
        if (!branchId.HasValue || branchId.Value != currentUser.BranchId.Value)
        {
            throw new ForbiddenException("You can only manage staff in your own branch.");
        }
    }

    private async Task<string?> GetPrimaryRoleAsync(User user, CancellationToken cancellationToken)
    {
        if (user.UserRoles.Count > 0)
        {
            var roleIds = user.UserRoles.Select(x => x.RoleId).ToArray();
            var roles = await _dbContext.Roles.AsNoTracking().Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToListAsync(cancellationToken);
            return roles.OrderBy(RoleRank).FirstOrDefault();
        }

        return null;
    }

    private sealed class StaffValidationException : AppException
    {
        public StaffValidationException(string message) : base(message) { }
    }
}
