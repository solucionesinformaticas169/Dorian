namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class DashboardEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SuperAdmin_Can_Get_Global_Dashboard_Summary()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/dashboard/summary");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<DashboardSummaryPayload>();
        payload.Should().NotBeNull();
        payload!.ActiveCustomersCount.Should().Be(6);
        payload.TodayClassesCount.Should().Be(0);
        payload.TodayCheckInsCount.Should().Be(0);
        payload.EstimatedRevenue.Should().Be(137m);
        payload.MostActiveBranchName.Should().Be("El Cebollar");
        payload.BranchActivity.Should().HaveCount(5);
        payload.ClassOccupancy.Should().BeEmpty();
    }

    [Fact]
    public async Task BranchAdmin_Sees_Only_Its_Branch_Dashboard_Summary()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/dashboard/summary");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<DashboardSummaryPayload>();
        payload.Should().NotBeNull();
        payload!.ActiveCustomersCount.Should().Be(5);
        payload.EstimatedRevenue.Should().Be(105m);
        payload.BranchActivity.Should().ContainSingle();
        payload.BranchActivity.Single().BranchName.Should().Be("El Cebollar");
    }

    [Fact]
    public async Task Customer_Cannot_Access_Dashboard_Summary()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/dashboard/summary");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record DashboardSummaryPayload(
        int ActiveCustomersCount,
        int TodayClassesCount,
        int TodayCheckInsCount,
        decimal EstimatedRevenue,
        string MostActiveBranchName,
        IReadOnlyCollection<BranchActivityPayload> BranchActivity,
        IReadOnlyCollection<ClassOccupancyPayload> ClassOccupancy);

    private sealed record BranchActivityPayload(
        Guid BranchId,
        string BranchName,
        int ActivityCount,
        int ActiveCustomersCount,
        int TodayClassesCount,
        int TodayCheckInsCount);

    private sealed record ClassOccupancyPayload(
        Guid ClassSessionId,
        string ClassName,
        string BranchName,
        DateTimeOffset StartTime,
        int ReservedSpots,
        int Capacity,
        decimal OccupancyRate);
}
