namespace Dorian.Api.Endpoints;

using Dorian.Application.Dashboard;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/dashboard").RequireAuthorization().WithTags("Dashboard");
        group.MapGet("/summary", async (IDashboardService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetSummaryAsync(cancellationToken)));
        return app;
    }
}
