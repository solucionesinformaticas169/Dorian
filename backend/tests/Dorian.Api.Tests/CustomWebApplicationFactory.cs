namespace Dorian.Api.Tests;

using System.Net.Http.Json;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Infrastructure.Persistence;
using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Dorian.Modules.Memberships.Domain.Entities;
using Dorian.Modules.Promotions.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"dorian-tests-{Guid.NewGuid()}";

    public Guid MainBranchId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public Guid SecondaryBranchId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public Guid SeededCustomerId { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public Guid SecondaryCustomerId { get; } = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public Guid MainBranchSecondCustomerId { get; } = Guid.Parse("12121212-1212-1212-1212-121212121212");
    public Guid TrainerUserId { get; } = Guid.Parse("13131313-1313-1313-1313-131313131313");
    public Guid ActiveGlobalPromotionId { get; } = Guid.Parse("15151515-1515-1515-1515-151515151515");
    public Guid DraftBranchPromotionId { get; } = Guid.Parse("16161616-1616-1616-1616-161616161616");
    public Guid ExpiredGlobalPromotionId { get; } = Guid.Parse("17171717-1717-1717-1717-171717171717");
    public Guid MainMembershipId { get; } = Guid.Parse("18181818-1818-1818-1818-181818181818");
    public Guid SecondaryMembershipId { get; } = Guid.Parse("19191919-1919-1919-1919-191919191919");
    public Guid InactiveCustomerId { get; } = Guid.Parse("20202020-2020-2020-2020-202020202020");
    public Guid ExpiredMembershipCustomerId { get; } = Guid.Parse("21212121-2121-2121-2121-212121212121");
    public Guid NoMembershipCustomerId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public Guid RevokedPassCustomerId { get; } = Guid.Parse("23232323-2323-2323-2323-232323232323");
    public string ValidAccessPassQr => "ACC-VALID-MAIN";
    public string ExpiredAccessPassQr => "ACC-EXPIRED-MAIN";
    public string RevokedAccessPassQr => "ACC-REVOKED-MAIN";
    public string SuperAdminEmail => "superadmin@dorian.test";
    public string BranchAdminEmail => "branchadmin@dorian.test";
    public string ReceptionEmail => "reception@dorian.test";
    public string TrainerEmail => "trainer@dorian.test";
    public string CustomerEmail => "customer@dorian.test";
    public string MainBranchSecondCustomerEmail => "customer2@dorian.test";
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

        dbContext.Memberships.AddRange(
            new Membership(MainMembershipId, MainBranchId, "Mensual Central", 30, 35m, "USD", true),
            new Membership(SecondaryMembershipId, SecondaryBranchId, "Mensual Sur", 30, 32m, "USD", true));

        var superAdmin = new User(Guid.NewGuid(), SuperAdminEmail, "Super Admin", passwordHasher.Hash(Password));
        superAdmin.SetRoles([SeedData.SuperAdminRoleId]);

        var branchAdmin = new User(Guid.NewGuid(), BranchAdminEmail, "Branch Admin", passwordHasher.Hash(Password));
        branchAdmin.AssignToBranch(MainBranchId);
        branchAdmin.SetRoles([SeedData.BranchAdminRoleId]);

        var reception = new User(Guid.NewGuid(), ReceptionEmail, "Reception User", passwordHasher.Hash(Password));
        reception.AssignToBranch(MainBranchId);
        reception.SetRoles([SeedData.ReceptionRoleId]);

        var trainer = new User(TrainerUserId, TrainerEmail, "Main Trainer", passwordHasher.Hash(Password));
        trainer.AssignToBranch(MainBranchId);
        trainer.SetRoles([SeedData.TrainerRoleId]);

        var customerUser = new User(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), CustomerEmail, "Jane Customer", passwordHasher.Hash(Password));
        customerUser.AssignToBranch(MainBranchId);
        customerUser.SetRoles([SeedData.CustomerRoleId]);

        var secondMainCustomerUser = new User(Guid.Parse("14141414-1414-1414-1414-141414141414"), MainBranchSecondCustomerEmail, "John Customer", passwordHasher.Hash(Password));
        secondMainCustomerUser.AssignToBranch(MainBranchId);
        secondMainCustomerUser.SetRoles([SeedData.CustomerRoleId]);

        var secondaryCustomerUser = new User(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), "othercustomer@dorian.test", "Other Customer", passwordHasher.Hash(Password));
        secondaryCustomerUser.AssignToBranch(SecondaryBranchId);
        secondaryCustomerUser.SetRoles([SeedData.CustomerRoleId]);

        var inactiveCustomerUser = new User(Guid.Parse("24242424-2424-2424-2424-242424242424"), "inactivecustomer@dorian.test", "Inactive Customer", passwordHasher.Hash(Password));
        inactiveCustomerUser.AssignToBranch(MainBranchId);
        inactiveCustomerUser.SetRoles([SeedData.CustomerRoleId]);
        inactiveCustomerUser.SetActive(false);

        var expiredMembershipCustomerUser = new User(Guid.Parse("25252525-2525-2525-2525-252525252525"), "expiredmembership@dorian.test", "Expired Membership Customer", passwordHasher.Hash(Password));
        expiredMembershipCustomerUser.AssignToBranch(MainBranchId);
        expiredMembershipCustomerUser.SetRoles([SeedData.CustomerRoleId]);

        var noMembershipCustomerUser = new User(Guid.Parse("26262626-2626-2626-2626-262626262626"), "nomembership@dorian.test", "No Membership Customer", passwordHasher.Hash(Password));
        noMembershipCustomerUser.AssignToBranch(MainBranchId);
        noMembershipCustomerUser.SetRoles([SeedData.CustomerRoleId]);

        var revokedPassCustomerUser = new User(Guid.Parse("27272727-2727-2727-2727-272727272727"), "revokedpass@dorian.test", "Revoked Pass Customer", passwordHasher.Hash(Password));
        revokedPassCustomerUser.AssignToBranch(MainBranchId);
        revokedPassCustomerUser.SetRoles([SeedData.CustomerRoleId]);

        dbContext.Users.AddRange(superAdmin, branchAdmin, reception, trainer, customerUser, secondMainCustomerUser, secondaryCustomerUser, inactiveCustomerUser, expiredMembershipCustomerUser, noMembershipCustomerUser, revokedPassCustomerUser);

        dbContext.Customers.AddRange(
            new Customer(SeededCustomerId, customerUser.Id, MainBranchId, "Jane", "Customer", "ID-001", "0991111111", new DateOnly(1995, 1, 1), Gender.Female, "Mom", "0992222222", MainMembershipId, DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow.AddDays(20), CustomerStatus.Active),
            new Customer(MainBranchSecondCustomerId, secondMainCustomerUser.Id, MainBranchId, "John", "Customer", "ID-003", "0993333333", new DateOnly(1994, 6, 6), Gender.Male, "Sister", "0994444444", MainMembershipId, DateTimeOffset.UtcNow.AddDays(-7), DateTimeOffset.UtcNow.AddDays(14), CustomerStatus.Active),
            new Customer(SecondaryCustomerId, secondaryCustomerUser.Id, SecondaryBranchId, "Other", "Customer", "ID-002", "0987777777", new DateOnly(1990, 1, 1), Gender.Male, "Dad", "0986666666", SecondaryMembershipId, DateTimeOffset.UtcNow.AddDays(-5), DateTimeOffset.UtcNow.AddDays(25), CustomerStatus.Active),
            new Customer(InactiveCustomerId, inactiveCustomerUser.Id, MainBranchId, "Inactive", "Customer", "ID-004", "0995550000", new DateOnly(1991, 4, 1), Gender.Female, "Brother", "0995551000", MainMembershipId, DateTimeOffset.UtcNow.AddDays(-4), DateTimeOffset.UtcNow.AddDays(15), CustomerStatus.Inactive),
            new Customer(ExpiredMembershipCustomerId, expiredMembershipCustomerUser.Id, MainBranchId, "Expired", "Membership", "ID-005", "0995552000", new DateOnly(1992, 8, 10), Gender.Male, "Spouse", "0995553000", MainMembershipId, DateTimeOffset.UtcNow.AddDays(-40), DateTimeOffset.UtcNow.AddDays(-1), CustomerStatus.Active),
            new Customer(NoMembershipCustomerId, noMembershipCustomerUser.Id, MainBranchId, "No", "Membership", "ID-006", "0995554000", new DateOnly(1993, 9, 15), Gender.Male, "Friend", "0995555000", null, CustomerStatus.Active),
            new Customer(RevokedPassCustomerId, revokedPassCustomerUser.Id, MainBranchId, "Revoked", "Pass", "ID-007", "0995556000", new DateOnly(1996, 11, 20), Gender.Female, "Parent", "0995557000", MainMembershipId, DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(10), CustomerStatus.Active));

        var validPass = new AccessPass(Guid.Parse("28282828-2828-2828-2828-282828282828"), SeededCustomerId, ValidAccessPassQr, DateTimeOffset.UtcNow.AddHours(8));
        var expiredPass = new AccessPass(Guid.Parse("29292929-2929-2929-2929-292929292929"), MainBranchSecondCustomerId, ExpiredAccessPassQr, DateTimeOffset.UtcNow.AddHours(-2));
        var revokedPass = new AccessPass(Guid.Parse("30303030-3030-3030-3030-303030303030"), RevokedPassCustomerId, RevokedAccessPassQr, DateTimeOffset.UtcNow.AddHours(8));
        revokedPass.Revoke();

        dbContext.AccessPasses.AddRange(
            validPass,
            expiredPass,
            revokedPass,
            new AccessPass(Guid.Parse("31313131-3131-3131-3131-313131313131"), SecondaryCustomerId, "ACC-VALID-SECONDARY", DateTimeOffset.UtcNow.AddHours(8)));

        dbContext.Promotions.AddRange(
            new Promotion(ActiveGlobalPromotionId, null, "Global Promo", "Visible global promotion", null, PromotionDiscountType.Informational, null, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(5), PromotionStatus.Active),
            new Promotion(DraftBranchPromotionId, MainBranchId, "Branch Draft", "Draft branch promotion", null, PromotionDiscountType.Percentage, 10m, DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(10), PromotionStatus.Draft),
            new Promotion(ExpiredGlobalPromotionId, null, "Expired Promo", "Expired promotion", null, PromotionDiscountType.FixedAmount, 5m, DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow.AddDays(-1), PromotionStatus.Expired));

        await dbContext.SaveChangesAsync();
    }

    private sealed record AuthResponsePayload(string AccessToken);
}
