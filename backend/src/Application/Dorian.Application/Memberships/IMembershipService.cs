namespace Dorian.Application.Memberships;

public interface IMembershipService
{
    Task<IReadOnlyCollection<MembershipResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<MembershipResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<MembershipResponse> CreateAsync(CreateMembershipRequest request, CancellationToken cancellationToken);
    Task<MembershipResponse> UpdateAsync(Guid id, UpdateMembershipRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
