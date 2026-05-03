using Dorian.Api.Endpoints;
using Dorian.Api.Extensions;
using Dorian.Application;
using Dorian.Infrastructure;
using Dorian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
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
                || origin.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase));
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
    if (dbContext.Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
    {
        await dbContext.Database.MigrateAsync();
    }
    else
    {
        await dbContext.Database.EnsureCreatedAsync();
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
app.MapBookingEndpoints();
app.MapPromotionEndpoints();
app.Run();

public partial class Program;
