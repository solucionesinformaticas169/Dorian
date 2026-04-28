namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class BranchesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BranchesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SuperAdmin_Can_Create_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/branches", new
        {
            code = "NORTE",
            name = "Sucursal Norte",
            city = "Quito",
            address = "Av. Norte",
            phoneNumber = "0988888888"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
