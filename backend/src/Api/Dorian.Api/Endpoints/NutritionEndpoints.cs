namespace Dorian.Api.Endpoints;

using Dorian.Application.Nutrition;

public static class NutritionEndpoints
{
    public static IEndpointRouteBuilder MapNutritionEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers").RequireAuthorization().WithTags("Nutrition");

        customers.MapGet("/me/nutrition-profile", async (INutritionService service, CancellationToken cancellationToken) =>
        {
            var profile = await service.GetMyProfileAsync(cancellationToken);
            return profile is null ? Results.NoContent() : Results.Ok(profile);
        });

        customers.MapPost("/me/nutrition-profile/generate", async (INutritionService service, CancellationToken cancellationToken) =>
            Results.Created("/customers/me/nutrition-profile", await service.GenerateMyProfileAsync(cancellationToken)));

        customers.MapPut("/me/nutrition-profile", async (UpdateNutritionProfileRequest request, INutritionService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateMyProfileAsync(request, cancellationToken)));

        customers.MapGet("/me/meal-plan", async (INutritionService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyMealPlanAsync(cancellationToken)));

        customers.MapPost("/me/meal-plan/generate", async (INutritionService service, CancellationToken cancellationToken) =>
            Results.Created("/customers/me/meal-plan", await service.GenerateMyMealPlanAsync(cancellationToken)));

        customers.MapGet("/{customerId:guid}/nutrition-profile", async (Guid customerId, INutritionService service, CancellationToken cancellationToken) =>
        {
            var profile = await service.GetProfileByCustomerIdAsync(customerId, cancellationToken);
            return profile is null ? Results.NoContent() : Results.Ok(profile);
        });

        customers.MapGet("/{customerId:guid}/meal-plan", async (Guid customerId, INutritionService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMealPlanByCustomerIdAsync(customerId, cancellationToken)));

        return app;
    }
}
