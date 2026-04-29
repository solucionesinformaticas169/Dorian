namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;

public sealed class MobileSupportEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MobileSupportEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customer_Can_Get_Own_Profile_For_Mobile_App()
    {
        using var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/customers/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CustomerPayload>();
        Assert.NotNull(payload);
        Assert.Equal(_factory.SeededCustomerId, payload!.Id);
        Assert.Equal(_factory.MainMembershipId, payload.ActiveMembershipId);
        Assert.Equal("Mensual Central", payload.ActiveMembershipName);
        Assert.Equal("USD", payload.ActiveMembershipCurrency);
    }

    [Fact]
    public async Task Customer_Can_List_Active_Branches_For_Mobile_App()
    {
        using var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/branches");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<BranchPayload>>();
        Assert.NotNull(payload);
        Assert.Contains(payload!, branch => branch.Id == _factory.MainBranchId);
        Assert.Contains(payload!, branch => branch.Id == _factory.SecondaryBranchId);
    }

    private sealed record CustomerPayload(Guid Id, Guid? ActiveMembershipId, string? ActiveMembershipName, string? ActiveMembershipCurrency);
    private sealed record BranchPayload(Guid Id, string Name);
}