namespace Dorian.Api.Endpoints;

using Dorian.Application.TrainingPlans;

public static class TrainingPlanEndpoints
{
    public static IEndpointRouteBuilder MapTrainingPlanEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers").RequireAuthorization().WithTags("Training Plans");
        var trainingDays = app.MapGroup("/training-days").RequireAuthorization().WithTags("Training Plans");
        var exercises = app.MapGroup("/exercises").RequireAuthorization().WithTags("Exercise Catalog");

        customers.MapGet("/me/training-plan", async (ITrainingPlanService service, CancellationToken cancellationToken) =>
        {
            var plan = await service.GetMyPlanAsync(cancellationToken);
            return plan is null ? Results.NoContent() : Results.Ok(plan);
        });

        customers.MapPost("/me/training-plan/generate", async (ITrainingPlanService service, CancellationToken cancellationToken) =>
            Results.Created("/customers/me/training-plan", await service.GenerateForCurrentCustomerAsync(cancellationToken)));

        customers.MapGet("/{customerId:guid}/training-plan", async (Guid customerId, ITrainingPlanService service, CancellationToken cancellationToken) =>
        {
            var plan = await service.GetByCustomerIdAsync(customerId, cancellationToken);
            return plan is null ? Results.NoContent() : Results.Ok(plan);
        });

        customers.MapPost("/{customerId:guid}/training-plan/generate", async (Guid customerId, ITrainingPlanService service, CancellationToken cancellationToken) =>
            Results.Created($"/customers/{customerId}/training-plan", await service.GenerateForCustomerAsync(customerId, cancellationToken)));

        trainingDays.MapPut("/{id:guid}/complete", async (Guid id, ITrainingPlanService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.CompleteDayAsync(id, cancellationToken)));

        trainingDays.MapPut("/{id:guid}/uncomplete", async (Guid id, ITrainingPlanService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UncompleteDayAsync(id, cancellationToken)));

        exercises.MapGet(string.Empty, async (string? muscleGroup, IExerciseCatalogService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListAsync(muscleGroup, cancellationToken)));

        exercises.MapGet("/{id:guid}", async (Guid id, IExerciseCatalogService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByIdAsync(id, cancellationToken)));

        return app;
    }
}
