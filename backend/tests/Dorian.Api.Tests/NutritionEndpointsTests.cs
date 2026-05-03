namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

public sealed class NutritionEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public NutritionEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Generates_Nutrition_Profile()
    {
        var client = await CreateAuthenticatedClientAsync("customer@dorian.test");
        await CreateFitnessProfileAsync(client);

        var response = await client.PostAsync("/customers/me/nutrition-profile/generate", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<NutritionProfileItem>();
        payload.Should().NotBeNull();
        payload!.DailyCaloriesTarget.Should().BeGreaterThan(1400);
        payload.Disclaimer.Should().Contain("referencial");
    }

    [Fact]
    public async Task Generates_Meal_Plan()
    {
        var client = await CreateAuthenticatedClientAsync("customer2@dorian.test");
        await CreateFitnessProfileAsync(client);
        await client.PostAsync("/customers/me/nutrition-profile/generate", null);

        var response = await client.PostAsync("/customers/me/meal-plan/generate", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<List<MealPlanItem>>();
        payload.Should().NotBeNull();
        var plans = payload!;
        plans.Should().NotBeEmpty();
        plans[0].Items.Should().HaveCountGreaterOrEqualTo(4);
    }

    [Fact]
    public async Task Calculates_Basic_Macros_Correctly()
    {
        var client = await CreateAuthenticatedClientAsync("nomembership@dorian.test");
        await CreateFitnessProfileAsync(client, goal: 3);

        var response = await client.PostAsync("/customers/me/nutrition-profile/generate", null);
        var payload = await response.Content.ReadFromJsonAsync<NutritionProfileItem>();

        payload.Should().NotBeNull();
        payload!.ProteinGrams.Should().BeGreaterThan(payload.FatGrams);
        payload.CarbsGrams.Should().BeGreaterThan(100);
        payload.WaterLitersTarget.Should().BeGreaterThan(2m);
    }

    [Fact]
    public async Task Customer_Cannot_Access_Nutrition_Of_Other_Customer()
    {
        var ownerClient = await CreateAuthenticatedClientAsync("expiredmembership@dorian.test");
        await CreateFitnessProfileAsync(ownerClient);
        await ownerClient.PostAsync("/customers/me/nutrition-profile/generate", null);

        var client = await CreateAuthenticatedClientAsync(_factory.MainBranchSecondCustomerEmail);
        var response = await client.GetAsync($"/customers/{_factory.ExpiredMembershipCustomerId}/nutrition-profile");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static Task<HttpResponseMessage> CreateFitnessProfileAsync(HttpClient client, int goal = 4)
        => client.PostAsJsonAsync("/customers/me/fitness-profile", new
        {
            goal,
            focusMuscleGroup = 1,
            experienceLevel = 2,
            gymType = 2,
            includeCardio = true,
            trainingDays = new[] { 1, 3, 5 },
            preferredTrainingTime = "18:30",
            gender = 1,
            birthDate = new DateOnly(1994, 4, 10),
            weightKg = 78m,
            heightCm = 176m,
            targetWeightKg = goal == 1 ? 72m : 82m,
            notificationsEnabled = true,
            notificationIntensity = 2,
            onboardingCompleted = true
        });

    private sealed record NutritionProfileItem(int DailyCaloriesTarget, int ProteinGrams, int CarbsGrams, int FatGrams, decimal WaterLitersTarget, string Disclaimer);
    private sealed record MealPlanItem(List<MealItem> Items);
    private sealed record MealItem(string Name);
}
