namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

public sealed class CustomersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SuperAdmin_Can_Create_Customer()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.SuperAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/customers", new
        {
            email = "newcustomer@dorian.test",
            password = _factory.Password,
            branchId = _factory.MainBranchId,
            activeMembershipId = (Guid?)null,
            firstName = "New",
            lastName = "Customer",
            identificationNumber = "ID-100",
            phone = "0990000000",
            birthDate = new DateOnly(1998, 5, 20),
            gender = 2,
            emergencyContactName = "Sister",
            emergencyContactPhone = "0991231234",
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BranchAdmin_Can_Create_Customer_In_Own_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/customers", new
        {
            email = "branchcustomer@dorian.test",
            password = _factory.Password,
            branchId = _factory.MainBranchId,
            activeMembershipId = (Guid?)null,
            firstName = "Branch",
            lastName = "Customer",
            identificationNumber = "ID-101",
            phone = "0993333333",
            birthDate = new DateOnly(1997, 3, 10),
            gender = 1,
            emergencyContactName = "Brother",
            emergencyContactPhone = "0994444444",
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BranchAdmin_Cannot_Manage_Customers_From_Another_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/customers", new
        {
            email = "forbiddencustomer@dorian.test",
            password = _factory.Password,
            branchId = _factory.SecondaryBranchId,
            activeMembershipId = (Guid?)null,
            firstName = "Forbidden",
            lastName = "Customer",
            identificationNumber = "ID-102",
            phone = "0995555555",
            birthDate = new DateOnly(1996, 8, 12),
            gender = 1,
            emergencyContactName = "Friend",
            emergencyContactPhone = "0996666666",
            status = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Can_List_Customers_By_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.ReceptionEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/branches/{_factory.MainBranchId}/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<List<CustomerItem>>();
        payload.Should().NotBeNull();
        payload!.Should().Contain(x => x.Id == _factory.SeededCustomerId);
    }

    [Fact]
    public async Task Customer_Can_Only_View_Own_Profile()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var ownResponse = await client.GetAsync($"/customers/{_factory.SeededCustomerId}");
        ownResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var otherResponse = await client.GetAsync($"/customers/{_factory.SecondaryCustomerId}");
        otherResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record CustomerItem(Guid Id);
}
