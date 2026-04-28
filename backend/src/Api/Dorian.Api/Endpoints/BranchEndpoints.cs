namespace Dorian.Api.Endpoints;

using Dorian.Api.Extensions;
using Dorian.Application.Branches;
using Dorian.Application.Classes;
using Dorian.Application.Customers;
using Dorian.Application.Promotions;
using FluentValidation;

public static class BranchEndpoints
{
    public static IEndpointRouteBuilder MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/branches").RequireAuthorization().WithTags("Branches");
        group.MapGet("/", async (IBranchService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, IBranchService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByIdAsync(id, cancellationToken)));
        group.MapGet("/{branchId:guid}/customers", async (Guid branchId, ICustomerService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByBranchAsync(branchId, cancellationToken)));
        group.MapGet("/{branchId:guid}/classes", async (Guid branchId, IClassSessionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByBranchAsync(branchId, cancellationToken)));
        group.MapGet("/{branchId:guid}/promotions", async (Guid branchId, IPromotionService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByBranchAsync(branchId, cancellationToken)));
        group.MapPost("/", async (CreateBranchRequest request, IValidator<CreateBranchRequest> validator, IBranchService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/branches/{response.Id}", response);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateBranchRequest request, IValidator<UpdateBranchRequest> validator, IBranchService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateAsync(id, request, cancellationToken));
        });
        group.MapDelete("/{id:guid}", async (Guid id, IBranchService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
