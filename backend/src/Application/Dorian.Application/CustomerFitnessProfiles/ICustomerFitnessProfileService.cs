namespace Dorian.Application.CustomerFitnessProfiles;

public interface ICustomerFitnessProfileService
{
    Task<CustomerFitnessProfileResponse> GetMeAsync(CancellationToken cancellationToken);
    Task<CustomerFitnessProfileResponse> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<CustomerFitnessProfileResponse> CreateForCurrentCustomerAsync(SaveCustomerFitnessProfileRequest request, CancellationToken cancellationToken);
    Task<CustomerFitnessProfileResponse> UpdateForCurrentCustomerAsync(SaveCustomerFitnessProfileRequest request, CancellationToken cancellationToken);
}
