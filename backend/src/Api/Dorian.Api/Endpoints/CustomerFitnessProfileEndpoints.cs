namespace Dorian.Api.Endpoints;

using Dorian.Application.CustomerFitnessProfiles;
using FluentValidation;

public static class CustomerFitnessProfileEndpoints
{
    public static IEndpointRouteBuilder MapCustomerFitnessProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/customers").RequireAuthorization().WithTags("Customer Fitness Profiles");

        group.MapGet("/me/fitness-profile", async (ICustomerFitnessProfileService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMeAsync(cancellationToken)));

        group.MapPost("/me/fitness-profile", async (SaveCustomerFitnessProfileRequest request, IValidator<SaveCustomerFitnessProfileRequest> validator, ICustomerFitnessProfileService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Created("/customers/me/fitness-profile", await service.CreateForCurrentCustomerAsync(request, cancellationToken));
        });

        group.MapPut("/me/fitness-profile", async (SaveCustomerFitnessProfileRequest request, IValidator<SaveCustomerFitnessProfileRequest> validator, ICustomerFitnessProfileService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateForCurrentCustomerAsync(request, cancellationToken));
        });

        group.MapGet("/{customerId:guid}/fitness-profile", async (Guid customerId, ICustomerFitnessProfileService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByCustomerIdAsync(customerId, cancellationToken)));

        return app;
    }
}
