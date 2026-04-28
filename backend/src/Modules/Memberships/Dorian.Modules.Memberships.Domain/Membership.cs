namespace Dorian.Modules.Memberships.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Membership : AuditableEntity<Guid>
{
    public Membership(Guid id, Guid branchId, string name, int durationInDays, decimal price) : base(id)
    {
        BranchId = branchId;
        Name = name;
        DurationInDays = durationInDays;
        Price = price;
        Currency = "USD";
        IsActive = true;
    }

    public Guid BranchId { get; private set; }

    public string Name { get; private set; }

    public int DurationInDays { get; private set; }

    public decimal Price { get; private set; }

    public string Currency { get; private set; }

    public bool IsActive { get; private set; }
}
