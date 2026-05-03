namespace Dorian.Api.Endpoints;

using Dorian.Application.GroupClasses;

public static class GroupClassEndpoints
{
    public static IEndpointRouteBuilder MapGroupClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/group-classes").WithTags("GroupClasses");
        group.MapGet("/", async (IGroupClassCatalogService service, CancellationToken cancellationToken) =>
                Results.Ok(await service.GetAllAsync(cancellationToken)))
            .AllowAnonymous();
        group.MapGet("/{slug}", async (string slug, IGroupClassCatalogService service, CancellationToken cancellationToken) =>
                Results.Ok(await service.GetBySlugAsync(slug, cancellationToken)))
            .AllowAnonymous();
        return app;
    }
}
