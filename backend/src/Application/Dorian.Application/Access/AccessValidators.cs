namespace Dorian.Application.Access;

using FluentValidation;

public sealed class ScanCheckInRequestValidator : AbstractValidator<ScanCheckInRequest>
{
    public ScanCheckInRequestValidator()
    {
        RuleFor(x => x.QrCodeValue).NotEmpty().MaximumLength(200);
    }
}

public sealed class ManualCheckInRequestValidator : AbstractValidator<ManualCheckInRequest>
{
    public ManualCheckInRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
