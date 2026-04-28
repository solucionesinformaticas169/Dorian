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

var app = builder.Build();
app.UseDorianExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Ok(new { name = "Dorian API", status = "ready", timestampUtc = DateTimeOffset.UtcNow })).AllowAnonymous();
app.MapHealthChecks("/health").AllowAnonymous();
app.MapAuthEndpoints();
app.MapBranchEndpoints();
app.MapMembershipEndpoints();
app.Run();

public partial class Program;
