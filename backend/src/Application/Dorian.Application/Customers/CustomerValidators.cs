namespace Dorian.Application.Customers;

using Dorian.Modules.Customers.Domain.Entities;
using FluentValidation;

public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.EmergencyContactName).MaximumLength(100);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.BirthDate).Must(x => x is null || x <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("BirthDate must be in the past.");
        CustomerMembershipValidationRules.ApplyMembershipWindowRules(this);
    }
}

public sealed class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.EmergencyContactName).MaximumLength(100);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.BirthDate).Must(x => x is null || x <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("BirthDate must be in the past.");
        CustomerMembershipValidationRules.ApplyMembershipWindowRules(this);
    }
}

file static class CustomerMembershipValidationRules
{
    public static void ApplyMembershipWindowRules(AbstractValidator<CreateCustomerRequest> validator)
    {
        validator.RuleFor(x => x)
            .Must(HasValidMembershipWindow)
            .WithMessage("Active plan dates are required when a plan is assigned, and end date must be greater than start date.");
    }

    public static void ApplyMembershipWindowRules(AbstractValidator<UpdateCustomerRequest> validator)
    {
        validator.RuleFor(x => x)
            .Must(HasValidMembershipWindow)
            .WithMessage("Active plan dates are required when a plan is assigned, and end date must be greater than start date.");
    }

    private static bool HasValidMembershipWindow(CreateCustomerRequest request)
        => HasValidMembershipWindow(request.ActiveMembershipId, request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc);

    private static bool HasValidMembershipWindow(UpdateCustomerRequest request)
        => HasValidMembershipWindow(request.ActiveMembershipId, request.ActiveMembershipStartsAtUtc, request.ActiveMembershipEndsAtUtc);

    private static bool HasValidMembershipWindow(Guid? membershipId, DateTimeOffset? startsAt, DateTimeOffset? endsAt)
    {
        if (!membershipId.HasValue)
        {
            return startsAt is null && endsAt is null;
        }

        return startsAt.HasValue && endsAt.HasValue && endsAt.Value > startsAt.Value;
    }
}

