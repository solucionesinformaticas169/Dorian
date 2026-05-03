namespace Dorian.Application.GroupClasses;

public interface IGroupClassCatalogService
{
    Task<IReadOnlyCollection<GroupClassCatalogItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<GroupClassCatalogItem> GetBySlugAsync(string slug, CancellationToken cancellationToken);
}
