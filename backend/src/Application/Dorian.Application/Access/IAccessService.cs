namespace Dorian.Application.Access;

public interface IAccessService
{
    Task<AccessPassResponse> GetAccessPassAsync(Guid customerId, CancellationToken cancellationToken);
    Task<AccessPassResponse> RegenerateAccessPassAsync(Guid customerId, CancellationToken cancellationToken);
    Task<CheckInResponse> ScanAsync(Guid branchId, ScanCheckInRequest request, CancellationToken cancellationToken);
    Task<CheckInResponse> ManualAsync(Guid branchId, ManualCheckInRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CheckInResponse>> GetBranchCheckInsAsync(Guid branchId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CheckInResponse>> GetCustomerCheckInsAsync(Guid customerId, CancellationToken cancellationToken);
}
