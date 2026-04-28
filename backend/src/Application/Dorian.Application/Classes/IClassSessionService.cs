namespace Dorian.Application.Classes;

public interface IClassSessionService
{
    Task<IReadOnlyCollection<ClassSessionResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ClassSessionResponse>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken);
    Task<ClassSessionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ClassSessionResponse> CreateAsync(CreateClassSessionRequest request, CancellationToken cancellationToken);
    Task<ClassSessionResponse> UpdateAsync(Guid id, UpdateClassSessionRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
