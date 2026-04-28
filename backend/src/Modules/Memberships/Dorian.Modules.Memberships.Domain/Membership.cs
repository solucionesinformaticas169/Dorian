namespace Dorian.Modules.Memberships.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Membership : AuditableEntity<Guid>
{
    private Membership() : base(Guid.Empty)
    {
    }

    public Membership(Guid id, Guid branchId, string name, int durationInDays, decimal price, string currency, bool isActive) : base(id)
    {
        Update(branchId, name, durationInDays, price, currency, isActive);
    }

    public Guid BranchId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int DurationInDays { get; private set; }

    public decimal Price { get; private set; }

    public string Currency { get; private set; } = "USD";

    public bool IsActive { get; private set; }

    public void Update(Guid branchId, string name, int durationInDays, decimal price, string currency, bool isActive)
    {
        BranchId = branchId;
        Name = name.Trim();
        DurationInDays = durationInDays;
        Price = price;
        Currency = currency.Trim().ToUpperInvariant();
        IsActive = isActive;
    }
}
