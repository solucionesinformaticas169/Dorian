namespace Dorian.Api.Endpoints;

using Dorian.Application.BodyTracking;
using FluentValidation;

public static class BodyTrackingEndpoints
{
    public static IEndpointRouteBuilder MapBodyTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/customers").RequireAuthorization().WithTags("Body Tracking");

        group.MapGet("/me/body-measurements", async (IBodyTrackingService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyMeasurementsAsync(cancellationToken)));

        group.MapGet("/me/body-measurements/latest", async (IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            var measurement = await service.GetMyLatestMeasurementAsync(cancellationToken);
            return measurement is null ? Results.NoContent() : Results.Ok(measurement);
        });

        group.MapPost("/me/body-measurements", async (SaveBodyMeasurementRequest request, IValidator<SaveBodyMeasurementRequest> validator, IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Created("/customers/me/body-measurements", await service.CreateMeasurementForCurrentCustomerAsync(request, cancellationToken));
        });

        group.MapPut("/me/body-measurements/{measurementId:guid}", async (Guid measurementId, SaveBodyMeasurementRequest request, IValidator<SaveBodyMeasurementRequest> validator, IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.UpdateMeasurementForCurrentCustomerAsync(measurementId, request, cancellationToken));
        });

        group.MapDelete("/me/body-measurements/{measurementId:guid}", async (Guid measurementId, IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteMeasurementForCurrentCustomerAsync(measurementId, cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/me/body-progress-photos", async (IBodyTrackingService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyProgressPhotosAsync(cancellationToken)));

        group.MapPost("/me/body-progress-photos", async (SaveBodyProgressPhotoRequest request, IValidator<SaveBodyProgressPhotoRequest> validator, IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Created("/customers/me/body-progress-photos", await service.CreateProgressPhotoForCurrentCustomerAsync(request, cancellationToken));
        });

        group.MapDelete("/me/body-progress-photos/{photoId:guid}", async (Guid photoId, IBodyTrackingService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteProgressPhotoForCurrentCustomerAsync(photoId, cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/me/body-summary", async (IBodyTrackingService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMyBodySummaryAsync(cancellationToken)));

        group.MapGet("/{customerId:guid}/body-measurements", async (Guid customerId, IBodyTrackingService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMeasurementsByCustomerIdAsync(customerId, cancellationToken)));

        group.MapGet("/{customerId:guid}/body-summary", async (Guid customerId, IBodyTrackingService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetBodySummaryByCustomerIdAsync(customerId, cancellationToken)));

        return app;
    }
}
