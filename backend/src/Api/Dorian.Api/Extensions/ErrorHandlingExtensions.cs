namespace Dorian.Api.Extensions;

using Dorian.Application.Abstractions.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseDorianExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(handlerApp =>
        {
            handlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception is null) return;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = exception switch
                {
                    ValidationException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    NotFoundException => StatusCodes.Status404NotFound,
                    AppException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message,
                    detail = exception is ValidationException validationException ? validationException.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }) : null
                });
            });
        });
    }
}
