namespace Dorian.Api.Endpoints;

using Dorian.Application.WorkoutActivities;

public static class WorkoutActivityEndpoints
{
    public static IEndpointRouteBuilder MapWorkoutActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers").RequireAuthorization().WithTags("Workout Activities");

        customers.MapGet("/me/activity-summary", async (int? range, IWorkoutActivityService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMySummaryAsync(range ?? 7, cancellationToken)));

        customers.MapGet("/me/activity-history", async (IWorkoutActivityService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyHistoryAsync(cancellationToken)));

        customers.MapGet("/me/muscle-activity", async (IWorkoutActivityService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyMuscleActivityAsync(cancellationToken)));

        customers.MapPost("/me/workout-activities", async (CreateWorkoutActivityRequest request, IWorkoutActivityService service, CancellationToken cancellationToken) =>
            Results.Created("/customers/me/activity-history", await service.CreateManualActivityAsync(request, cancellationToken)));

        customers.MapGet("/{customerId:guid}/activity-summary", async (Guid customerId, int? range, IWorkoutActivityService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetSummaryByCustomerIdAsync(customerId, range ?? 7, cancellationToken)));

        return app;
    }
}
