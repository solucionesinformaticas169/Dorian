namespace Dorian.Application.Staff;

using Dorian.Modules.Identity.Domain.Constants;
using FluentValidation;

public sealed class CreateStaffMemberRequestValidator : AbstractValidator<CreateStaffMemberRequest>
{
    public CreateStaffMemberRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Role).Must(BeSupportedRole).WithMessage("Only BranchAdmin, Reception or Trainer can be managed from this module.");
        RuleFor(x => x.BranchId)
            .NotNull()
            .When(x => x.Role is RoleNames.BranchAdmin or RoleNames.Reception or RoleNames.Trainer)
            .WithMessage("A branch is required for this staff role.");
    }

    private static bool BeSupportedRole(string role)
        => role is RoleNames.BranchAdmin or RoleNames.Reception or RoleNames.Trainer;
}

public sealed class UpdateStaffMemberRequestValidator : AbstractValidator<UpdateStaffMemberRequest>
{
    public UpdateStaffMemberRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Role).Must(BeSupportedRole).WithMessage("Only BranchAdmin, Reception or Trainer can be managed from this module.");
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
        RuleFor(x => x.BranchId)
            .NotNull()
            .When(x => x.Role is RoleNames.BranchAdmin or RoleNames.Reception or RoleNames.Trainer)
            .WithMessage("A branch is required for this staff role.");
    }

    private static bool BeSupportedRole(string role)
        => role is RoleNames.BranchAdmin or RoleNames.Reception or RoleNames.Trainer;
}
