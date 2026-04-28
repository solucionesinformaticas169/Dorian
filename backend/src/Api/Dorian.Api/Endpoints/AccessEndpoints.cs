namespace Dorian.Api.Endpoints;

using Dorian.Application.Access;
using FluentValidation;

public static class AccessEndpoints
{
    public static IEndpointRouteBuilder MapAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/{customerId:guid}/access-pass", async (Guid customerId, IAccessService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAccessPassAsync(customerId, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Access");

        app.MapPost("/customers/{customerId:guid}/access-pass/regenerate", async (Guid customerId, IAccessService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.RegenerateAccessPassAsync(customerId, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Access");

        app.MapPost("/branches/{branchId:guid}/check-ins/scan", async (Guid branchId, ScanCheckInRequest request, IValidator<ScanCheckInRequest> validator, IAccessService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.ScanAsync(branchId, request, cancellationToken));
        }).RequireAuthorization().WithTags("Access");

        app.MapPost("/branches/{branchId:guid}/check-ins/manual", async (Guid branchId, ManualCheckInRequest request, IValidator<ManualCheckInRequest> validator, IAccessService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.ManualAsync(branchId, request, cancellationToken));
        }).RequireAuthorization().WithTags("Access");

        app.MapGet("/branches/{branchId:guid}/check-ins", async (Guid branchId, IAccessService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetBranchCheckInsAsync(branchId, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Access");

        app.MapGet("/customers/{customerId:guid}/check-ins", async (Guid customerId, IAccessService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetCustomerCheckInsAsync(customerId, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Access");

        return app;
    }
}
