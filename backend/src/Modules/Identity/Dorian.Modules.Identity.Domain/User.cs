namespace Dorian.Modules.Identity.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class User : AuditableEntity<Guid>
{
    private readonly HashSet<Guid> _roleIds = [];

    public User(Guid id, string email, string fullName) : base(id)
    {
        Email = email;
        FullName = fullName;
        IsActive = true;
    }

    public string Email { get; private set; }

    public string FullName { get; private set; }

    public string? PhoneNumber { get; private set; }

    public Guid? BranchId { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Guid> RoleIds => _roleIds;

    public void AssignToBranch(Guid branchId)
    {
        BranchId = branchId;
    }

    public void AddRole(Guid roleId)
    {
        _roleIds.Add(roleId);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
