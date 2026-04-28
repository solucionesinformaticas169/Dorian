namespace Dorian.Application.Promotions;

public interface IPromotionService
{
    Task<IReadOnlyCollection<PromotionResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PromotionResponse>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken);
    Task<PromotionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken);
    Task<PromotionResponse> UpdateAsync(Guid id, UpdatePromotionRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<PromotionResponse> ActivateAsync(Guid id, CancellationToken cancellationToken);
    Task<PromotionResponse> DisableAsync(Guid id, CancellationToken cancellationToken);
}
