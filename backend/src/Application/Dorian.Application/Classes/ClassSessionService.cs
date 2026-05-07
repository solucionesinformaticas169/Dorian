namespace Dorian.Application.Classes;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Classes.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class ClassSessionService : IClassSessionService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ClassSessionService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<ClassSessionResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        IQueryable<ClassSession> query = _dbContext.ClassSessions.AsNoTracking();

        if (user.IsInRole(RoleNames.SuperAdmin))
            return await Project(query).ToListAsync(cancellationToken);

        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId.HasValue)
            return await Project(query.Where(x => x.BranchId == user.BranchId.Value)).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.Trainer) && user.UserId.HasValue)
            return await Project(query.Where(x => x.TrainerUserId == user.UserId.Value)).ToListAsync(cancellationToken);

        if (user.IsInRole(RoleNames.Customer))
            return await Project(query.Where(x => x.Status == ClassSessionStatus.Scheduled)).ToListAsync(cancellationToken);

        throw new ForbiddenException("You do not have access to classes.");
    }

    public async Task<IReadOnlyCollection<ClassSessionResponse>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken)
    {
        EnsureCanViewBranch(branchId);
        return await Project(_dbContext.ClassSessions.AsNoTracking().Where(x => x.BranchId == branchId)).ToListAsync(cancellationToken);
    }

    public async Task<ClassSessionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var classSession = await _dbContext.ClassSessions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Class not found.");
        EnsureCanViewClass(classSession);
        return await ProjectSingle(classSession, cancellationToken);
    }

    public async Task<ClassSessionResponse> CreateAsync(CreateClassSessionRequest request, CancellationToken cancellationToken)
    {
        EnsureCanManageBranch(request.BranchId, allowReception: false);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        await EnsureTrainerValid(request.BranchId, request.TrainerUserId, cancellationToken);

        var classSession = new ClassSession(Guid.NewGuid(), request.BranchId, request.TrainerUserId, request.Name, request.Description, request.StartTime, request.EndTime, request.Capacity, request.Status);
        _dbContext.ClassSessions.Add(classSession);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await ProjectSingle(classSession, cancellationToken);
    }

    public async Task<ClassSessionResponse> UpdateAsync(Guid id, UpdateClassSessionRequest request, CancellationToken cancellationToken)
    {
        var classSession = await _dbContext.ClassSessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Class not found.");
        EnsureCanManageBranch(classSession.BranchId, allowReception: false);
        EnsureCanManageBranch(request.BranchId, allowReception: false);
        await EnsureBranchExists(request.BranchId, cancellationToken);
        await EnsureTrainerValid(request.BranchId, request.TrainerUserId, cancellationToken);

        classSession.Update(request.BranchId, request.TrainerUserId, request.Name, request.Description, request.StartTime, request.EndTime, request.Capacity, request.Status);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await ProjectSingle(classSession, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var classSession = await _dbContext.ClassSessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException("Class not found.");
        EnsureCanManageBranch(classSession.BranchId, allowReception: false);
        _dbContext.ClassSessions.Remove(classSession);
        await _dbContext.SaveChangesAsync(cancellationToken);
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
        if (user.IsInRole(RoleNames.Customer)) return;
        throw new ForbiddenException("You cannot view classes for this branch.");
    }

    private void EnsureCanViewClass(ClassSession classSession)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception)) && user.BranchId == classSession.BranchId) return;
        if (user.IsInRole(RoleNames.Customer)) return;
        if (user.IsInRole(RoleNames.Trainer) && user.UserId == classSession.TrainerUserId) return;
        throw new ForbiddenException("You cannot view this class.");
    }

    private void EnsureCanManageBranch(Guid branchId, bool allowReception)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.BranchAdmin) && user.BranchId == branchId) return;
        if (allowReception && user.IsInRole(RoleNames.Reception) && user.BranchId == branchId) return;
        throw new ForbiddenException("You cannot manage classes for this branch.");
    }

    private async Task EnsureBranchExists(Guid branchId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Branches.AnyAsync(x => x.Id == branchId, cancellationToken))
            throw new NotFoundException("Branch not found.");
    }

    private async Task EnsureTrainerValid(Guid branchId, Guid? trainerUserId, CancellationToken cancellationToken)
    {
        if (!trainerUserId.HasValue) return;
        var isValid = await _dbContext.Users
            .Include(x => x.UserRoles)
            .AnyAsync(x => x.Id == trainerUserId.Value && x.BranchId == branchId && x.UserRoles.Any(r => r.Role.Name == RoleNames.Trainer), cancellationToken);
        if (!isValid) throw new NotFoundException("Trainer not found for the selected branch.");
    }

    private IQueryable<ClassSessionResponse> Project(IQueryable<ClassSession> query)
    {
        return query
            .OrderBy(x => x.StartTime)
            .Select(x => new ClassSessionResponse(
                x.Id,
                x.BranchId,
                x.TrainerUserId,
                x.Name,
                x.Description,
                x.StartTime,
                x.EndTime,
                x.Capacity,
                x.Status,
                _dbContext.Bookings.Count(b => b.ClassSessionId == x.Id && b.Status == BookingStatus.Reserved),
                x.CreatedAtUtc,
                x.UpdatedAtUtc));
    }

    private Task<ClassSessionResponse> ProjectSingle(ClassSession classSession, CancellationToken cancellationToken)
    {
        return Project(_dbContext.ClassSessions.AsNoTracking().Where(x => x.Id == classSession.Id)).SingleAsync(cancellationToken);
    }
}
