namespace Dorian.Application.BodyTracking;

using Dorian.Modules.Customers.Domain.Entities;
using FluentValidation;

public sealed class SaveBodyMeasurementRequestValidator : AbstractValidator<SaveBodyMeasurementRequest>
{
    public SaveBodyMeasurementRequestValidator()
    {
        RuleFor(x => x.WeightKg).GreaterThan(0);
        RuleFor(x => x.HeightCm).GreaterThan(0);
        RuleFor(x => x.MeasuredAt)
            .Must(value => value <= DateTimeOffset.UtcNow)
            .WithMessage("Measurement date cannot be in the future.");

        RuleFor(x => x.BodyFatPercentage).InclusiveBetween(0, 100).When(x => x.BodyFatPercentage.HasValue);
        RequireNonNegative(x => x.MuscleMassKg);
        RequireNonNegative(x => x.BoneMassKg);
        RequireNonNegative(x => x.ResidualMassKg);
        RequireNonNegative(x => x.WaistCm);
        RequireNonNegative(x => x.ChestCm);
        RequireNonNegative(x => x.HipCm);
        RequireNonNegative(x => x.ShouldersCm);
        RequireNonNegative(x => x.LeftArmCm);
        RequireNonNegative(x => x.RightArmCm);
        RequireNonNegative(x => x.LeftLegCm);
        RequireNonNegative(x => x.RightLegCm);
        RequireNonNegative(x => x.LeftCalfCm);
        RequireNonNegative(x => x.RightCalfCm);
        RequireNonNegative(x => x.NeckCm);
    }

    private void RequireNonNegative(System.Linq.Expressions.Expression<Func<SaveBodyMeasurementRequest, decimal?>> expression)
    {
        RuleFor(expression)
            .GreaterThanOrEqualTo(0)
            .When(request => expression.Compile().Invoke(request).HasValue);
    }
}

public sealed class SaveBodyProgressPhotoRequestValidator : AbstractValidator<SaveBodyProgressPhotoRequest>
{
    public SaveBodyProgressPhotoRequestValidator()
    {
        RuleFor(x => x.PhotoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TakenAt)
            .Must(value => value <= DateTimeOffset.UtcNow)
            .WithMessage("Photo date cannot be in the future.");
        RuleFor(x => x.Type).IsInEnum();
    }
}
