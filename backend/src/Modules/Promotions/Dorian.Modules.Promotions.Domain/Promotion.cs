namespace Dorian.Modules.Promotions.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Promotion : AuditableEntity<Guid>
{
    public Promotion(Guid id, string code, string title, PromotionDiscountType discountType, decimal value) : base(id)
    {
        Code = code;
        Title = title;
        DiscountType = discountType;
        Value = value;
        IsActive = true;
    }

    public string Code { get; private set; }

    public string Title { get; private set; }

    public PromotionDiscountType DiscountType { get; private set; }

    public decimal Value { get; private set; }

    public DateTimeOffset? ValidFromUtc { get; private set; }

    public DateTimeOffset? ValidToUtc { get; private set; }

    public bool IsActive { get; private set; }
}
