namespace Dorian.Api.Endpoints;

using Dorian.Application.Bookings;
using FluentValidation;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/bookings", async (IBookingService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)))
            .RequireAuthorization()
            .WithTags("Bookings");

        app.MapGet("/customers/{customerId:guid}/bookings", async (Guid customerId, IBookingService service, CancellationToken cancellationToken) => Results.Ok(await service.GetByCustomerAsync(customerId, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Bookings");

        app.MapPost("/classes/{classId:guid}/bookings", async (Guid classId, CreateBookingRequest request, IValidator<CreateBookingRequest> validator, IBookingService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.CreateAsync(classId, request, cancellationToken);
            return Results.Created($"/bookings/{response.Id}", response);
        }).RequireAuthorization().WithTags("Bookings");

        app.MapPut("/bookings/{id:guid}/cancel", async (Guid id, IBookingService service, CancellationToken cancellationToken) => Results.Ok(await service.CancelAsync(id, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Bookings");

        app.MapPut("/bookings/{id:guid}/attend", async (Guid id, IBookingService service, CancellationToken cancellationToken) => Results.Ok(await service.AttendAsync(id, cancellationToken)))
            .RequireAuthorization()
            .WithTags("Bookings");

        return app;
    }
}
