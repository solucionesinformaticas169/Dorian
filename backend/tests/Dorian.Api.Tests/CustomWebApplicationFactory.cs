namespace Dorian.Api.Tests;

using System.Net.Http.Json;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Infrastructure.Persistence;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"dorian-tests-{Guid.NewGuid()}";

    public Guid MainBranchId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public string SuperAdminEmail => "superadmin@dorian.test";
    public string BranchAdminEmail => "branchadmin@dorian.test";
    public string Password => "Pass1234!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IDorianDbContext>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.AddScoped<IDorianDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IAppPasswordHasher>();

            dbContext.Database.EnsureCreated();
            SeedDataAsync(dbContext, passwordHasher).GetAwaiter().GetResult();
        });
    }

    public async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new { email, password });
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{(int)response.StatusCode}: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthResponsePayload>();
        return payload!.AccessToken;
    }

    private async Task SeedDataAsync(AppDbContext dbContext, IAppPasswordHasher passwordHasher)
    {
        if (await dbContext.Users.AnyAsync()) return;

        if (!await dbContext.Roles.AnyAsync())
        {
            dbContext.Roles.AddRange(
                new Role(SeedData.CustomerRoleId, RoleNames.Customer, "Gym customer"),
                new Role(SeedData.TrainerRoleId, RoleNames.Trainer, "Gym trainer"),
                new Role(SeedData.ReceptionRoleId, RoleNames.Reception, "Front desk staff"),
                new Role(SeedData.BranchAdminRoleId, RoleNames.BranchAdmin, "Branch administrator"),
                new Role(SeedData.SuperAdminRoleId, RoleNames.SuperAdmin, "Platform super administrator"));
        }

        var branch = new Branch(MainBranchId, "CENTRAL", "Sucursal Central", "Quito", "Av. Principal", "0999999999");
        dbContext.Branches.Add(branch);

        var superAdmin = new User(Guid.NewGuid(), SuperAdminEmail, "Super Admin", passwordHasher.Hash(Password));
        superAdmin.SetRoles([SeedData.SuperAdminRoleId]);

        var branchAdmin = new User(Guid.NewGuid(), BranchAdminEmail, "Branch Admin", passwordHasher.Hash(Password));
        branchAdmin.AssignToBranch(MainBranchId);
        branchAdmin.SetRoles([SeedData.BranchAdminRoleId]);

        dbContext.Users.AddRange(superAdmin, branchAdmin);
        await dbContext.SaveChangesAsync();
    }

    private sealed record AuthResponsePayload(string AccessToken);
}
