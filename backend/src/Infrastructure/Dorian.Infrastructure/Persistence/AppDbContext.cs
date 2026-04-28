namespace Dorian.Infrastructure.Persistence;

using System.Text.Json;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Auditing.Domain.Entities;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Identity.Domain.Entities;
using Dorian.Modules.Memberships.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext, IDorianDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditLogs = PrepareAuditLogs();
        if (auditLogs.Count > 0) AuditLogs.AddRange(auditLogs);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private List<AuditLog> PrepareAuditLogs()
    {
        var actor = _currentUserService?.User;
        var entries = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(x => x.Entity is not AuditLog)
            .Where(x => ShouldAudit(x.Entity.GetType()))
            .ToArray();

        var logs = new List<AuditLog>();
        foreach (var entry in entries)
        {
            var idProperty = entry.Properties.FirstOrDefault(x => x.Metadata.Name == "Id");
            var entityId = idProperty?.CurrentValue?.ToString() ?? idProperty?.OriginalValue?.ToString() ?? string.Empty;
            var action = entry.State switch
            {
                EntityState.Added => "Create",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };

            var changed = entry.Properties
                .Where(x => entry.State != EntityState.Modified || x.IsModified)
                .ToDictionary(
                    x => x.Metadata.Name,
                    x => new { Original = entry.State == EntityState.Added ? null : x.OriginalValue, Current = entry.State == EntityState.Deleted ? null : x.CurrentValue });

            logs.Add(new AuditLog(Guid.NewGuid(), actor?.UserId, actor?.BranchId, action, entry.Entity.GetType().Name, entityId, JsonSerializer.Serialize(changed)));
        }

        return logs;
    }

    private static bool ShouldAudit(Type entityType) => entityType == typeof(User) || entityType == typeof(Branch) || entityType == typeof(Membership);
}
