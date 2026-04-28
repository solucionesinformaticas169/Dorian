namespace Dorian.Application.Branches;

public interface IBranchService
{
    Task<IReadOnlyCollection<BranchResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<BranchResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken);
    Task<BranchResponse> UpdateAsync(Guid id, UpdateBranchRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
