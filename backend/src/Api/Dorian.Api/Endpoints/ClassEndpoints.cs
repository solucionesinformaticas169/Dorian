namespace Dorian.Api.Endpoints;

using Dorian.Application.Classes;
using FluentValidation;

public static class ClassEndpoints
{
    public static IEndpointRouteBuilder MapClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/classes").RequireAuthorization().WithTags("Classes");
        group.MapGet("/", async (IClassSessionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, IClassSessionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapPost("/", async (CreateClassSessionRequest request, IValidator<CreateClassSessionRequest> validator, IClassSessionService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/classes/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateClassSessionRequest request, IValidator<UpdateClassSessionRequest> validator, IClassSessionService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, IClassSessionService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
