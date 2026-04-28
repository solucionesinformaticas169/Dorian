namespace Dorian.Application.Classes;

using FluentValidation;

public sealed class CreateClassSessionRequestValidator : AbstractValidator<CreateClassSessionRequest>
{
    public CreateClassSessionRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateClassSessionRequestValidator : AbstractValidator<UpdateClassSessionRequest>
{
    public UpdateClassSessionRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        RuleFor(x => x.Status).IsInEnum();
    }
}
