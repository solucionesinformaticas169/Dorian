namespace Dorian.Api.Tests;

using FluentAssertions;

public sealed class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_Returns_Access_And_Refresh_Tokens()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/auth/login", new { email = _factory.SuperAdminEmail, password = _factory.Password });
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed with {(int)response.StatusCode}: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    private sealed record LoginResponse(string AccessToken, string RefreshToken);
}
