namespace Dorian.Api.Endpoints;

using Dorian.Api.Extensions;
using Dorian.Application.Staff;
using FluentValidation;

public static class StaffEndpoints
{
    public static IEndpointRouteBuilder MapStaffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/staff").RequireAuthorization().WithTags("Staff");
        group.MapGet("/", async (IStaffService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, IStaffService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapPost("/", async (CreateStaffMemberRequest request, IValidator<CreateStaffMemberRequest> validator, IStaffService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/staff/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateStaffMemberRequest request, IValidator<UpdateStaffMemberRequest> validator, IStaffService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, IStaffService service, CancellationToken cancellationToken) =>
        {
            await service.DisableAsync(id, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
