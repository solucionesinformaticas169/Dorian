namespace Dorian.Api.Endpoints;

using Dorian.Application.Promotions;
using FluentValidation;

public static class PromotionEndpoints
{
    public static IEndpointRouteBuilder MapPromotionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/promotions").RequireAuthorization().WithTags("Promotions");
        group.MapGet("/", async (IPromotionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, IPromotionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapPost("/", async (CreatePromotionRequest request, IValidator<CreatePromotionRequest> validator, IPromotionService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/promotions/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdatePromotionRequest request, IValidator<UpdatePromotionRequest> validator, IPromotionService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, IPromotionService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });
        group.MapPut("/{id:guid}/activate", async (Guid id, IPromotionService service, CancellationToken cancellationToken) => Results.Ok(await service.ActivateAsync(id, cancellationToken)));
        group.MapPut("/{id:guid}/disable", async (Guid id, IPromotionService service, CancellationToken cancellationToken) => Results.Ok(await service.DisableAsync(id, cancellationToken)));
        return app;
    }
}
