namespace Dorian.Api.Endpoints;

using Dorian.Application.Customers;
using FluentValidation;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/customers").RequireAuthorization().WithTags("Customers");
        group.MapGet("/", async (ICustomerService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, ICustomerService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapPost("/", async (CreateCustomerRequest request, IValidator<CreateCustomerRequest> validator, ICustomerService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/customers/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateCustomerRequest request, IValidator<UpdateCustomerRequest> validator, ICustomerService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, ICustomerService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
