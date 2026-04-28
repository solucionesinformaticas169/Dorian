namespace Dorian.Application.Promotions;

using Dorian.Modules.Promotions.Domain.Entities;

public sealed record PromotionResponse(
    Guid Id,
    Guid? BranchId,
    string Title,
    string Description,
    string? ImageUrl,
    PromotionDiscountType DiscountType,
    decimal? DiscountValue,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    PromotionStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CreatePromotionRequest(
    Guid? BranchId,
    string Title,
    string Description,
    string? ImageUrl,
    PromotionDiscountType DiscountType,
    decimal? DiscountValue,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    PromotionStatus Status);

public sealed record UpdatePromotionRequest(
    Guid? BranchId,
    string Title,
    string Description,
    string? ImageUrl,
    PromotionDiscountType DiscountType,
    decimal? DiscountValue,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    PromotionStatus Status);
