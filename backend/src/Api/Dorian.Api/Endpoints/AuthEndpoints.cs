namespace Dorian.Api.Endpoints;

using Dorian.Api.Extensions;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");
        group.MapPost("/register", async (RegisterRequest request, IValidator<RegisterRequest> validator, IAuthService service, HttpContext context, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.RegisterAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken));
        }).AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, IValidator<LoginRequest> validator, IAuthService service, HttpContext context, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.LoginAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken));
        }).AllowAnonymous();

        group.MapPost("/refresh", async (RefreshRequest request, IValidator<RefreshRequest> validator, IAuthService service, HttpContext context, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            return Results.Ok(await service.RefreshAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken));
        }).AllowAnonymous();

        group.MapPost("/logout", [Authorize] async (LogoutRequest request, IValidator<LogoutRequest> validator, ICurrentUserService currentUserService, IAuthService service, CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            if (!currentUserService.User.UserId.HasValue) return Results.Unauthorized();
            await service.LogoutAsync(currentUserService.User.UserId.Value, request, cancellationToken);
            return Results.NoContent();
        });
        return app;
    }
}
