namespace Dorian.Application.Promotions;

using Dorian.Modules.Promotions.Domain.Entities;
using FluentValidation;

public sealed class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.DiscountType).IsInEnum();
        RuleFor(x => x).Custom((request, context) => ValidateDiscount(request.DiscountType, request.DiscountValue, context));
    }

    private static void ValidateDiscount(PromotionDiscountType discountType, decimal? discountValue, ValidationContext<CreatePromotionRequest> context)
    {
        switch (discountType)
        {
            case PromotionDiscountType.Percentage:
                if (!discountValue.HasValue || discountValue <= 0 || discountValue > 100)
                    context.AddFailure(nameof(CreatePromotionRequest.DiscountValue), "Percentage promotions require DiscountValue between 0 and 100.");
                break;
            case PromotionDiscountType.FixedAmount:
                if (!discountValue.HasValue || discountValue <= 0)
                    context.AddFailure(nameof(CreatePromotionRequest.DiscountValue), "FixedAmount promotions require a positive DiscountValue.");
                break;
            case PromotionDiscountType.Informational:
                if (discountValue.HasValue)
                    context.AddFailure(nameof(CreatePromotionRequest.DiscountValue), "Informational promotions cannot have DiscountValue.");
                break;
        }
    }
}

public sealed class UpdatePromotionRequestValidator : AbstractValidator<UpdatePromotionRequest>
{
    public UpdatePromotionRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.DiscountType).IsInEnum();
        RuleFor(x => x).Custom((request, context) => ValidateDiscount(request.DiscountType, request.DiscountValue, context));
    }

    private static void ValidateDiscount(PromotionDiscountType discountType, decimal? discountValue, ValidationContext<UpdatePromotionRequest> context)
    {
        switch (discountType)
        {
            case PromotionDiscountType.Percentage:
                if (!discountValue.HasValue || discountValue <= 0 || discountValue > 100)
                    context.AddFailure(nameof(UpdatePromotionRequest.DiscountValue), "Percentage promotions require DiscountValue between 0 and 100.");
                break;
            case PromotionDiscountType.FixedAmount:
                if (!discountValue.HasValue || discountValue <= 0)
                    context.AddFailure(nameof(UpdatePromotionRequest.DiscountValue), "FixedAmount promotions require a positive DiscountValue.");
                break;
            case PromotionDiscountType.Informational:
                if (discountValue.HasValue)
                    context.AddFailure(nameof(UpdatePromotionRequest.DiscountValue), "Informational promotions cannot have DiscountValue.");
                break;
        }
    }
}
