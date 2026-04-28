namespace Dorian.Modules.Branches.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Branch : AuditableEntity<Guid>
{
    private Branch() : base(Guid.Empty)
    {
    }

    public Branch(Guid id, string code, string name, string city, string? address, string? phoneNumber) : base(id)
    {
        Update(code, name, city, address, phoneNumber, true);
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string? Address { get; private set; }

    public string? PhoneNumber { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(string code, string name, string city, string? address, string? phoneNumber, bool isActive)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        City = city.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        IsActive = isActive;
    }
}
