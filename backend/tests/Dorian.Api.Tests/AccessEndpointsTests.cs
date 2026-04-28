namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class AccessEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AccessEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customer_Can_Get_Own_Qr()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/customers/{_factory.SeededCustomerId}/access-pass");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<AccessPassDto>();
        payload.Should().NotBeNull();
        payload!.CustomerId.Should().Be(_factory.SeededCustomerId);
        payload.Status.Should().Be(1);
    }

    [Fact]
    public async Task Reception_Can_Accept_Valid_Qr()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/scan", new { qrCodeValue = _factory.ValidAccessPassQr });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<CheckInDto>();
        payload.Should().NotBeNull();
        payload!.Status.Should().Be(1);
        payload.BranchId.Should().Be(_factory.MainBranchId);
        payload.CustomerId.Should().Be(_factory.SeededCustomerId);
    }

    [Fact]
    public async Task Reception_Rejects_Inactive_Customer()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/manual", new { customerId = _factory.InactiveCustomerId });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<CheckInDto>();
        payload.Should().NotBeNull();
        payload!.Status.Should().Be(2);
        payload.RejectionReason.Should().Contain("not active");
    }

    [Fact]
    public async Task Reception_Rejects_Expired_Or_Missing_Membership()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var expiredResponse = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/manual", new { customerId = _factory.ExpiredMembershipCustomerId });
        expiredResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var expiredPayload = await expiredResponse.Content.ReadFromJsonAsync<CheckInDto>();
        expiredPayload.Should().NotBeNull();
        expiredPayload!.Status.Should().Be(2);
        expiredPayload.RejectionReason.Should().Contain("expired");

        var missingResponse = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/manual", new { customerId = _factory.NoMembershipCustomerId });
        missingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var missingPayload = await missingResponse.Content.ReadFromJsonAsync<CheckInDto>();
        missingPayload.Should().NotBeNull();
        missingPayload!.Status.Should().Be(2);
        missingPayload.RejectionReason.Should().Contain("active membership assigned");
    }

    [Fact]
    public async Task Reception_Rejects_Expired_Or_Revoked_Qr()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var expiredResponse = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/scan", new { qrCodeValue = _factory.ExpiredAccessPassQr });
        expiredResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var expiredPayload = await expiredResponse.Content.ReadFromJsonAsync<CheckInDto>();
        expiredPayload.Should().NotBeNull();
        expiredPayload!.Status.Should().Be(2);
        expiredPayload.RejectionReason.Should().Contain("expired");

        var revokedResponse = await client.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/scan", new { qrCodeValue = _factory.RevokedAccessPassQr });
        revokedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var revokedPayload = await revokedResponse.Content.ReadFromJsonAsync<CheckInDto>();
        revokedPayload.Should().NotBeNull();
        revokedPayload!.Status.Should().Be(2);
        revokedPayload.RejectionReason.Should().Contain("revoked");
    }

    [Fact]
    public async Task BranchAdmin_Can_Only_View_CheckIns_From_Own_Branch()
    {
        var superAdminClient = _factory.CreateClient();
        var superAdminToken = await _factory.LoginAsync(superAdminClient, _factory.SuperAdminEmail, _factory.Password);
        superAdminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        var mainCheckInResponse = await superAdminClient.PostAsJsonAsync($"/branches/{_factory.MainBranchId}/check-ins/manual", new { customerId = _factory.SeededCustomerId });
        mainCheckInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondaryCheckInResponse = await superAdminClient.PostAsJsonAsync($"/branches/{_factory.SecondaryBranchId}/check-ins/manual", new { customerId = _factory.SecondaryCustomerId });
        secondaryCheckInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var branchAdminClient = _factory.CreateClient();
        var branchAdminToken = await _factory.LoginAsync(branchAdminClient, _factory.BranchAdminEmail, _factory.Password);
        branchAdminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", branchAdminToken);

        var ownBranchResponse = await branchAdminClient.GetAsync($"/branches/{_factory.MainBranchId}/check-ins");
        ownBranchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var ownBranchPayload = await ownBranchResponse.Content.ReadFromJsonAsync<List<CheckInDto>>();
        ownBranchPayload.Should().NotBeNull();
        ownBranchPayload!.Should().Contain(x => x.CustomerId == _factory.SeededCustomerId && x.BranchId == _factory.MainBranchId);
        ownBranchPayload.Should().NotContain(x => x.CustomerId == _factory.SecondaryCustomerId && x.BranchId == _factory.SecondaryBranchId);

        var otherBranchResponse = await branchAdminClient.GetAsync($"/branches/{_factory.SecondaryBranchId}/check-ins");
        otherBranchResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record AccessPassDto(Guid CustomerId, int Status);
    private sealed record CheckInDto(Guid CustomerId, Guid BranchId, int Status, string? RejectionReason);
}
