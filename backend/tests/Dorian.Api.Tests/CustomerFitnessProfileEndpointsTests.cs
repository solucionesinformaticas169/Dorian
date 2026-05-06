namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dorian.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

public sealed class CustomerFitnessProfileEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomerFitnessProfileEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customer_Gets_OnboardingCompleted_False_When_Profile_Does_Not_Exist()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/customers/me/fitness-profile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<FitnessProfileItem>();
        payload.Should().NotBeNull();
        payload!.OnboardingCompleted.Should().BeFalse();
        payload.TrainingDays.Should().BeEmpty();
    }

    [Fact]
    public async Task Customer_Can_Create_Fitness_Profile()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.MainBranchSecondCustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/customers/me/fitness-profile", CreatePayload("18:30"));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<FitnessProfileItem>();
        payload.Should().NotBeNull();
        payload!.OnboardingCompleted.Should().BeTrue();
        payload.Goal.Should().Be(3);
        payload.TrainingDays.Should().BeEquivalentTo([1, 3, 5]);
    }

    [Fact]
    public async Task Customer_Can_Create_Fitness_Profile_Even_If_Customer_Record_Was_Missing()
    {
        var client = _factory.CreateClient();
        var email = $"regression+{Guid.NewGuid():N}@dorian.test";

        var registerResponse = await client.PostAsJsonAsync("/auth/register", new
        {
            fullName = "Regression Customer",
            email,
            password = _factory.Password,
            phoneNumber = "0991234567"
        });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = dbContext.Users.Single(x => x.Email == email);
            var customer = dbContext.Customers.Single(x => x.UserId == user.Id);
            dbContext.Customers.Remove(customer);
            await dbContext.SaveChangesAsync();
        }

        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/customers/me/fitness-profile", CreatePayload("18:30"));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<FitnessProfileItem>();
        payload.Should().NotBeNull();
        payload!.OnboardingCompleted.Should().BeTrue();
        payload.CustomerId.Should().NotBeNullOrEmpty();

        using var verificationScope = _factory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<AppDbContext>();
        verificationDb.Customers.Count(x => x.User.Email == email).Should().Be(1);
    }

    [Fact]
    public async Task Customer_Can_Update_Fitness_Profile()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await client.PostAsJsonAsync("/customers/me/fitness-profile", CreatePayload("18:30"));

        var response = await client.PutAsJsonAsync("/customers/me/fitness-profile", CreatePayload(null, goal: 2, targetWeightKg: 61m, trainingDays: [2, 4, 6]));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<FitnessProfileItem>();
        payload.Should().NotBeNull();
        payload!.Goal.Should().Be(2);
        payload.TargetWeightKg.Should().Be(61m);
        payload.TrainingDays.Should().BeEquivalentTo([2, 4, 6]);
    }

    [Fact]
    public async Task Customer_Cannot_Access_Profile_Of_Other_Customer()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.CustomerEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/customers/{_factory.SecondaryCustomerId}/fitness-profile");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BranchAdmin_Can_View_Profile_Of_Customer_In_Own_Branch()
    {
        var customerClient = _factory.CreateClient();
        var customerToken = await _factory.LoginAsync(customerClient, _factory.CustomerEmail, _factory.Password);
        customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", customerToken);
        await customerClient.PostAsJsonAsync("/customers/me/fitness-profile", CreatePayload("06:00", focusMuscleGroup: 2));

        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/customers/{_factory.SeededCustomerId}/fitness-profile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<FitnessProfileItem>();
        payload.Should().NotBeNull();
        payload!.OnboardingCompleted.Should().BeTrue();
        payload.FocusMuscleGroup.Should().Be(2);
    }

    [Fact]
    public async Task BranchAdmin_Cannot_View_Profile_Of_Other_Branch()
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/customers/{_factory.SecondaryCustomerId}/fitness-profile");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static object CreatePayload(string? preferredTrainingTime, int goal = 3, int focusMuscleGroup = 1, decimal targetWeightKg = 68m, int[]? trainingDays = null) => new
    {
        goal,
        focusMuscleGroup,
        experienceLevel = 2,
        gymType = 2,
        includeCardio = true,
        trainingDays = trainingDays ?? [1, 3, 5],
        preferredTrainingTime,
        gender = 2,
        birthDate = new DateOnly(1995, 5, 20),
        weightKg = 72m,
        heightCm = 168m,
        targetWeightKg,
        notificationsEnabled = true,
        notificationIntensity = 2,
        onboardingCompleted = true
    };

    private sealed record FitnessProfileItem(
        string? CustomerId,
        bool OnboardingCompleted,
        int? Goal,
        int? FocusMuscleGroup,
        decimal? TargetWeightKg,
        List<int> TrainingDays);
}
