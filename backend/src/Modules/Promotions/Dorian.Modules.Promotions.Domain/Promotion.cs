namespace Dorian.Modules.Promotions.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Promotion : AuditableEntity<Guid>
{
    private Promotion() : base(Guid.Empty)
    {
    }

    public Promotion(
        Guid id,
        Guid? branchId,
        string title,
        string description,
        string? imageUrl,
        PromotionDiscountType discountType,
        decimal? discountValue,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        PromotionStatus status) : base(id)
    {
        Update(branchId, title, description, imageUrl, discountType, discountValue, startsAt, endsAt, status);
    }

    public Guid? BranchId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public PromotionDiscountType DiscountType { get; private set; }
    public decimal? DiscountValue { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public PromotionStatus Status { get; private set; }

    public void Update(
        Guid? branchId,
        string title,
        string description,
        string? imageUrl,
        PromotionDiscountType discountType,
        decimal? discountValue,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        PromotionStatus status)
    {
        BranchId = branchId;
        Title = title.Trim();
        Description = description.Trim();
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        DiscountType = discountType;
        DiscountValue = discountValue;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Status = status;
    }

    public void Activate()
    {
        Status = PromotionStatus.Active;
    }

    public void Disable()
    {
        Status = PromotionStatus.Disabled;
    }
}
