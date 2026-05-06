namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class PromotionsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PromotionsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SuperAdmin_Can_Create_Global_Promotion()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/promotions", new
        {
            branchId = (Guid?)null,
            title = "Global Summer Promo",
            description = "Applies to all branches",
            imageUrl = "https://example.com/promo.png",
            discountType = 3,
            discountValue = (decimal?)null,
            startsAt = DateTimeOffset.UtcNow.AddDays(1),
            endsAt = DateTimeOffset.UtcNow.AddDays(10),
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BranchAdmin_Can_Create_Promotion_For_Own_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/promotions", new
        {
            branchId = _factory.MainBranchId,
            title = "Branch Promo",
            description = "Only main branch",
            imageUrl = (string?)null,
            discountType = 1,
            discountValue = 15m,
            startsAt = DateTimeOffset.UtcNow.AddDays(1),
            endsAt = DateTimeOffset.UtcNow.AddDays(7),
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BranchAdmin_Cannot_Create_Global_Promotion()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/promotions", new
        {
            branchId = (Guid?)null,
            title = "Forbidden Global Promo",
            description = "Should fail",
            imageUrl = (string?)null,
            discountType = 3,
            discountValue = (decimal?)null,
            startsAt = DateTimeOffset.UtcNow.AddDays(1),
            endsAt = DateTimeOffset.UtcNow.AddDays(7),
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Customer_Only_Sees_Active_And_Current_Promotions()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/promotions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<List<PromotionItem>>();
        payload.Should().NotBeNull();
        payload!.Should().ContainSingle(x => x.Id == _factory.ActiveGlobalPromotionId);
        payload.Should().NotContain(x => x.Id == _factory.DraftBranchPromotionId);
        payload.Should().NotContain(x => x.Id == _factory.ExpiredGlobalPromotionId);
    }

    [Fact]
    public async Task Newly_Registered_Customer_Without_Branch_Still_Sees_Global_Active_Promotions()
    {
        var client = _factory.CreateClient();
        var email = $"promo-new-{Guid.NewGuid():N}@dorian.test";

        var registerResponse = await client.PostAsJsonAsync("/auth/register", new
        {
            email,
            fullName = "Promo New Customer",
            password = _factory.Password
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/promotions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<List<PromotionItem>>();
        payload.Should().NotBeNull();
        payload!.Should().Contain(x => x.Id == _factory.ActiveGlobalPromotionId);
    }

    [Fact]
    public async Task SuperAdmin_Can_Activate_And_Disable_Promotion()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var activateResponse = await client.PutAsync($"/promotions/{_factory.DraftBranchPromotionId}/activate", null);
        activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var activated = await activateResponse.Content.ReadFromJsonAsync<PromotionItem>();
        activated!.Status.Should().Be(2);

        var disableResponse = await client.PutAsync($"/promotions/{_factory.DraftBranchPromotionId}/disable", null);
        disableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var disabled = await disableResponse.Content.ReadFromJsonAsync<PromotionItem>();
        disabled!.Status.Should().Be(4);
    }

    [Fact]
    public async Task Invalid_Dates_Are_Rejected()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/promotions", new
        {
            branchId = (Guid?)null,
            title = "Invalid Dates Promo",
            description = "Should fail validation",
            imageUrl = (string?)null,
            discountType = 3,
            discountValue = (decimal?)null,
            startsAt = DateTimeOffset.UtcNow.AddDays(10),
            endsAt = DateTimeOffset.UtcNow.AddDays(1),
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record PromotionItem(Guid Id, int Status);
}
