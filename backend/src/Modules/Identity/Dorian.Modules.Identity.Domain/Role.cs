namespace Dorian.Modules.Identity.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Role : AuditableEntity<Guid>
{
    private readonly HashSet<string> _permissions = [];

    public Role(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyCollection<string> Permissions => _permissions;

    public void GrantPermission(string permission)
    {
        _permissions.Add(permission);
    }
}
