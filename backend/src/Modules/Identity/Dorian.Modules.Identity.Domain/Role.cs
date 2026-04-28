namespace Dorian.Modules.Identity.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Role : AuditableEntity<Guid>
{
    private readonly List<UserRole> _userRoles = [];

    private Role() : base(Guid.Empty)
    {
    }

    public Role(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
}
