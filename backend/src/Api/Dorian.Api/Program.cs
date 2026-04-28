var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    name = "Dorian API",
    status = "bootstrapped",
    timestampUtc = DateTimeOffset.UtcNow
}));

app.MapHealthChecks("/health");

app.Run();
