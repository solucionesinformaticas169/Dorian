using Dorian.Api.Endpoints;
using Dorian.Api.Extensions;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application;
using Dorian.Infrastructure;
using Dorian.Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var railwayPort = Environment.GetEnvironmentVariable("PORT");
var aspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

if (!string.IsNullOrWhiteSpace(railwayPort) && string.IsNullOrWhiteSpace(aspNetCoreUrls))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{railwayPort}");
}

var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, ".app-data-protection");
Directory.CreateDirectory(dataProtectionPath);
var configuredCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
var environmentCorsOrigins = (builder.Configuration["ALLOWED_ORIGINS"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
var allowedCorsOrigins = configuredCorsOrigins
    .Concat(environmentCorsOrigins)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddHealthChecks();
builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("Dorian");
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDorianSwagger();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentWeb", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase)
                || origin.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase)
                || allowedCorsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase));
    });
});

var app = builder.Build();
app.UseDorianExceptionHandling();
app.UseCors("DevelopmentWeb");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IAppPasswordHasher>();
    var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres") ?? string.Empty;
    if (dbContext.Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
    {
        await dbContext.Database.MigrateAsync();
    }
    else
    {
        await dbContext.Database.EnsureCreatedAsync();
    }

    var shouldSeedDemoData = !app.Environment.IsEnvironment("Testing")
        && (postgresConnectionString.Contains("Host=localhost", StringComparison.OrdinalIgnoreCase)
            || postgresConnectionString.Contains("Host=127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || postgresConnectionString.Contains("Server=localhost", StringComparison.OrdinalIgnoreCase));

    if (shouldSeedDemoData)
    {
        await DevelopmentDemoDataSeeder.SeedAsync(dbContext, passwordHasher, CancellationToken.None);
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Ok(new { name = "Dorian API", status = "ready", timestampUtc = DateTimeOffset.UtcNow })).AllowAnonymous();
app.MapHealthChecks("/health").AllowAnonymous();
app.MapAuthEndpoints();
app.MapAccessEndpoints();
app.MapBranchEndpoints();
app.MapClassEndpoints();
app.MapDashboardEndpoints();
app.MapGroupClassEndpoints();
app.MapMembershipEndpoints();
app.MapCustomerEndpoints();
app.MapCustomerFitnessProfileEndpoints();
app.MapBodyTrackingEndpoints();
app.MapNutritionEndpoints();
app.MapTrainingPlanEndpoints();
app.MapWorkoutActivityEndpoints();
app.MapBookingEndpoints();
app.MapPromotionEndpoints();
app.MapStaffEndpoints();
app.Run();

public partial class Program;
