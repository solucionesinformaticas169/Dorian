namespace Dorian.Modules.Identity.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class User : AuditableEntity<Guid>
{
    private readonly List<UserRole> _userRoles = [];
    private readonly List<RefreshToken> _refreshTokens = [];

    private User() : base(Guid.Empty)
    {
    }

    public User(Guid id, string email, string fullName, string passwordHash) : base(id)
    {
        Email = email.Trim().ToLowerInvariant();
        FullName = fullName.Trim();
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string? PhoneNumber { get; private set; }

    public Guid? BranchId { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    public void UpdateProfile(string fullName, string? phoneNumber)
    {
        FullName = fullName.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AssignToBranch(Guid? branchId)
    {
        BranchId = branchId;
    }

    public void SetRoles(IEnumerable<Guid> roleIds)
    {
        _userRoles.Clear();
        foreach (var roleId in roleIds.Distinct())
        {
            _userRoles.Add(new UserRole(Id, roleId));
        }
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
