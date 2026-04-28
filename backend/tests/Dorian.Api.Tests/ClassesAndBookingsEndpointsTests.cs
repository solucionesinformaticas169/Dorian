namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class ClassesAndBookingsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ClassesAndBookingsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task BranchAdmin_Can_Create_Class()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/classes", new
        {
            branchId = _factory.MainBranchId,
            trainerUserId = _factory.TrainerUserId,
            name = "Spinning",
            description = "Morning class",
            startTime = DateTimeOffset.UtcNow.AddDays(1),
            endTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            capacity = 10,
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BranchAdmin_Cannot_Create_Class_In_Another_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/classes", new
        {
            branchId = _factory.SecondaryBranchId,
            trainerUserId = (Guid?)null,
            name = "Yoga",
            description = "Forbidden branch",
            startTime = DateTimeOffset.UtcNow.AddDays(1),
            endTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            capacity = 10,
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Customer_Can_Reserve_Class()
    {
        var classId = await CreateClassAsBranchAdminAsync(10);
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"/classes/{classId}/bookings", new { customerId = _factory.SeededCustomerId });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Customer_Cannot_Create_Double_Reservation()
    {
        var classId = await CreateClassAsBranchAdminAsync(10);
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var first = await client.PostAsJsonAsync($"/classes/{classId}/bookings", new { customerId = _factory.SeededCustomerId });
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync($"/classes/{classId}/bookings", new { customerId = _factory.SeededCustomerId });
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Customer_Cannot_Reserve_If_Class_Is_Full()
    {
        var classId = await CreateClassAsBranchAdminAsync(1);

        await ReserveAsCustomerAsync(_factory.CustomerEmail, _factory.SeededCustomerId, classId);

        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.MainBranchSecondCustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var second = await client.PostAsJsonAsync($"/classes/{classId}/bookings", new { customerId = _factory.MainBranchSecondCustomerId });
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Customer_Can_Cancel_Own_Booking()
    {
        var classId = await CreateClassAsBranchAdminAsync(10);
        var bookingId = await ReserveAsCustomerAsync(_factory.CustomerEmail, _factory.SeededCustomerId, classId);

        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsync($"/bookings/{bookingId}/cancel", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Reception_Can_Mark_Attendance()
    {
        var classId = await CreateClassAsBranchAdminAsync(10);
        var bookingId = await ReserveAsCustomerAsync(_factory.CustomerEmail, _factory.SeededCustomerId, classId);

        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsync($"/bookings/{bookingId}/attend", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateClassAsBranchAdminAsync(int capacity)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/classes", new
        {
            branchId = _factory.MainBranchId,
            trainerUserId = _factory.TrainerUserId,
            name = $"Class-{Guid.NewGuid():N}",
            description = "Generated class",
            startTime = DateTimeOffset.UtcNow.AddDays(1),
            endTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            capacity,
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<ClassItem>();
        return payload!.Id;
    }

    private async Task<Guid> ReserveAsCustomerAsync(string email, Guid customerId, Guid classId)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"/classes/{classId}/bookings", new { customerId });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<BookingItem>();
        return payload!.Id;
    }

    private sealed record ClassItem(Guid Id);
    private sealed record BookingItem(Guid Id);
}
