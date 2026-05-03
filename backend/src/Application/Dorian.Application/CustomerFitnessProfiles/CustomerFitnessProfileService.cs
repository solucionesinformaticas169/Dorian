namespace Dorian.Application.CustomerFitnessProfiles;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class CustomerFitnessProfileService : ICustomerFitnessProfileService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CustomerFitnessProfileService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerFitnessProfileResponse> GetMeAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var profile = await _dbContext.CustomerFitnessProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);
        return profile is null ? CreateEmptyResponse(customer.Id) : Map(profile);
    }

    public async Task<CustomerFitnessProfileResponse> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");
        EnsureCanViewCustomer(customer);

        var profile = await _dbContext.CustomerFitnessProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        return profile is null ? CreateEmptyResponse(customer.Id) : Map(profile);
    }

    public async Task<CustomerFitnessProfileResponse> CreateForCurrentCustomerAsync(SaveCustomerFitnessProfileRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        if (await _dbContext.CustomerFitnessProfiles.AnyAsync(x => x.CustomerId == customer.Id, cancellationToken))
        {
            throw new FitnessProfileValidationException("Fitness profile already exists for this customer.");
        }

        var profile = new CustomerFitnessProfile(
            Guid.NewGuid(),
            customer.Id,
            request.Goal,
            request.FocusMuscleGroup,
            request.ExperienceLevel,
            request.GymType,
            request.IncludeCardio,
            request.TrainingDays,
            request.PreferredTrainingTime,
            request.Gender,
            request.BirthDate,
            request.WeightKg,
            request.HeightCm,
            request.TargetWeightKg,
            request.NotificationsEnabled,
            request.NotificationIntensity,
            request.OnboardingCompleted);

        _dbContext.CustomerFitnessProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    public async Task<CustomerFitnessProfileResponse> UpdateForCurrentCustomerAsync(SaveCustomerFitnessProfileRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var profile = await _dbContext.CustomerFitnessProfiles.SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NotFoundException("Fitness profile not found.");

        profile.Update(
            request.Goal,
            request.FocusMuscleGroup,
            request.ExperienceLevel,
            request.GymType,
            request.IncludeCardio,
            request.TrainingDays,
            request.PreferredTrainingTime,
            request.Gender,
            request.BirthDate,
            request.WeightKg,
            request.HeightCm,
            request.TargetWeightKg,
            request.NotificationsEnabled,
            request.NotificationIntensity,
            request.OnboardingCompleted);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    private async Task<Customer> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.Customer) || !user.UserId.HasValue)
        {
            throw new ForbiddenException("Only customers can manage their own fitness onboarding.");
        }

        return await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == user.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Customer profile not found.");
    }

    private void EnsureCanViewCustomer(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception) || user.IsInRole(RoleNames.Trainer)) && user.BranchId == customer.BranchId) return;
        throw new ForbiddenException("You cannot view this customer's fitness onboarding.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static CustomerFitnessProfileResponse CreateEmptyResponse(Guid customerId) => new(
        null,
        customerId,
        null,
        null,
        null,
        null,
        false,
        [],
        null,
        null,
        null,
        null,
        null,
        null,
        false,
        null,
        false,
        null,
        null);

    private static CustomerFitnessProfileResponse Map(CustomerFitnessProfile profile) => new(
        profile.Id,
        profile.CustomerId,
        profile.Goal,
        profile.FocusMuscleGroup,
        profile.ExperienceLevel,
        profile.GymType,
        profile.IncludeCardio,
        profile.GetTrainingDays(),
        profile.PreferredTrainingTime,
        profile.Gender,
        profile.BirthDate,
        profile.WeightKg,
        profile.HeightCm,
        profile.TargetWeightKg,
        profile.NotificationsEnabled,
        profile.NotificationIntensity,
        profile.OnboardingCompleted,
        profile.CreatedAtUtc,
        profile.UpdatedAtUtc);

    private sealed class FitnessProfileValidationException : AppException
    {
        public FitnessProfileValidationException(string message) : base(message)
        {
        }
    }
}
