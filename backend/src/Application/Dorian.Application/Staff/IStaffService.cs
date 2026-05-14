namespace Dorian.Application.Staff;

public interface IStaffService
{
    Task<IReadOnlyCollection<StaffMemberResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<StaffMemberResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StaffMemberResponse> CreateAsync(CreateStaffMemberRequest request, CancellationToken cancellationToken);
    Task<StaffMemberResponse> UpdateAsync(Guid id, UpdateStaffMemberRequest request, CancellationToken cancellationToken);
    Task DisableAsync(Guid id, CancellationToken cancellationToken);
}
