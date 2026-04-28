namespace Dorian.Api.Endpoints;

using Dorian.Api.Extensions;
using Dorian.Application.Memberships;
using FluentValidation;

public static class MembershipEndpoints
{
    public static IEndpointRouteBuilder MapMembershipEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/memberships").RequireAuthorization().WithTags("Memberships");
        group.MapGet("/", async (IMembershipService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, IMembershipService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapPost("/", async (CreateMembershipRequest request, IValidator<CreateMembershipRequest> validator, IMembershipService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/memberships/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateMembershipRequest request, IValidator<UpdateMembershipRequest> validator, IMembershipService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, IMembershipService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
