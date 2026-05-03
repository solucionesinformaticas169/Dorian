namespace Dorian.Application.CustomerFitnessProfiles;

using Dorian.Modules.Customers.Domain.Entities;
using FluentValidation;

public sealed class SaveCustomerFitnessProfileRequestValidator : AbstractValidator<SaveCustomerFitnessProfileRequest>
{
    public SaveCustomerFitnessProfileRequestValidator()
    {
        RuleFor(x => x.WeightKg).GreaterThan(0);
        RuleFor(x => x.HeightCm).GreaterThan(0);
        RuleFor(x => x.TargetWeightKg).GreaterThan(0);
        RuleFor(x => x.TrainingDays).NotEmpty();
        RuleForEach(x => x.TrainingDays).IsInEnum();
        RuleFor(x => x.BirthDate).Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Birth date cannot be in the future.");
        RuleFor(x => x.PreferredTrainingTime)
            .Must(value => string.IsNullOrWhiteSpace(value) || TimeOnly.TryParse(value, out _))
            .WithMessage("Preferred training time must be a valid time.");
        RuleFor(x => x.Goal).IsInEnum();
        RuleFor(x => x.FocusMuscleGroup).IsInEnum();
        RuleFor(x => x.ExperienceLevel).IsInEnum();
        RuleFor(x => x.GymType).IsInEnum();
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.NotificationIntensity).IsInEnum();
    }
}
