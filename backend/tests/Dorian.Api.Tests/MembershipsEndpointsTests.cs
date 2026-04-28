namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class MembershipsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MembershipsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task BranchAdmin_Can_Create_Membership_For_Own_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/memberships", new
        {
            branchId = _factory.MainBranchId,
            name = "Mensual",
            durationInDays = 30,
            price = 29.99m,
            currency = "USD",
            isActive = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
