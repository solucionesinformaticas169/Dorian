namespace Dorian.Modules.Branches.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Branch : AuditableEntity<Guid>
{
    public Branch(Guid id, string code, string name, string city) : base(id)
    {
        Code = code;
        Name = name;
        City = city;
        IsActive = true;
    }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string City { get; private set; }

    public string? Address { get; private set; }

    public string? PhoneNumber { get; private set; }

    public bool IsActive { get; private set; }
}
