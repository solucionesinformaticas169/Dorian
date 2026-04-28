namespace Dorian.Application.Bookings;

using FluentValidation;

public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
