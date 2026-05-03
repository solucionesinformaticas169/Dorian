namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

public sealed class WorkoutActivityEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public WorkoutActivityEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Completing_Training_Day_Generates_Workout_Activity()
    {
        var client = await CreateAuthenticatedClientAsync("customer@dorian.test");
        await EnsureProfileAndPlanAsync(client, new[] { 1, 3, 5 });
        var dayId = await GetFirstTrainingDayIdAsync(client);

        await client.PutAsync($"/training-days/{dayId}/complete", null);
        var historyResponse = await client.GetAsync("/customers/me/activity-history");

        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await historyResponse.Content.ReadFromJsonAsync<List<ActivityHistoryItem>>();
        history.Should().NotBeNull();
        history!.Should().ContainSingle(x => x.Title != "Actividad manual");
    }

    [Fact]
    public async Task Summary_For_Seven_Days_Is_Correct()
    {
        var client = await CreateAuthenticatedClientAsync("customer2@dorian.test");
        await EnsureProfileAndPlanAsync(client, new[] { 2, 4, 6 });
        var dayId = await GetFirstTrainingDayIdAsync(client);

        await client.PutAsync($"/training-days/{dayId}/complete", null);
        await client.PostAsJsonAsync("/customers/me/workout-activities", new
        {
            completedAt = DateTimeOffset.UtcNow.AddDays(-1),
            durationSeconds = 1800,
            caloriesEstimated = 220,
            notes = "Cardio libre",
            exercises = new[]
            {
                new { exerciseName = "Saltar cuerda", muscleGroup = 9, sets = 3, reps = "60", weightKg = (decimal?)null, completed = true }
            }
        });

        var response = await client.GetAsync("/customers/me/activity-summary?range=7");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<ActivitySummaryItem>();
        payload.Should().NotBeNull();
        payload!.DaysTrained.Should().BeGreaterThanOrEqualTo(2);
        payload.ActivityByDay.Should().HaveCount(7);
        payload.CaloriesEstimated.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task History_Returns_Manual_And_Automatic_Activities()
    {
        var client = await CreateAuthenticatedClientAsync("revokedpass@dorian.test");
        await EnsureProfileAndPlanAsync(client, new[] { 1, 4, 6 });
        var dayId = await GetFirstTrainingDayIdAsync(client);

        await client.PutAsync($"/training-days/{dayId}/complete", null);
        await client.PostAsJsonAsync("/customers/me/workout-activities", new
        {
            completedAt = DateTimeOffset.UtcNow,
            durationSeconds = 2400,
            caloriesEstimated = 260,
            notes = "Sesión rápida",
            exercises = new[]
            {
                new { exerciseName = "Remo con banda", muscleGroup = 2, sets = 4, reps = "12", weightKg = (decimal?)null, completed = true }
            }
        });

        var response = await client.GetAsync("/customers/me/activity-history");
        var payload = await response.Content.ReadFromJsonAsync<List<ActivityHistoryItem>>();

        payload.Should().NotBeNull();
        payload!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Customer_Cannot_Access_Activity_Of_Other_Customer()
    {
        var ownerClient = await CreateAuthenticatedClientAsync("nomembership@dorian.test");
        await EnsureProfileAndPlanAsync(ownerClient, new[] { 1, 2, 5 });
        var dayId = await GetFirstTrainingDayIdAsync(ownerClient);
        await ownerClient.PutAsync($"/training-days/{dayId}/complete", null);

        var client = await CreateAuthenticatedClientAsync(_factory.MainBranchSecondCustomerEmail);
        var response = await client.GetAsync($"/customers/{_factory.SeededCustomerId}/activity-summary?range=7");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static Task<HttpResponseMessage> CreateFitnessProfileAsync(HttpClient client, int[] trainingDays)
        => client.PostAsJsonAsync("/customers/me/fitness-profile", new
        {
            goal = 3,
            focusMuscleGroup = 1,
            experienceLevel = 2,
            gymType = 2,
            includeCardio = true,
            trainingDays,
            preferredTrainingTime = "18:30",
            gender = 2,
            birthDate = new DateOnly(1995, 5, 20),
            weightKg = 72m,
            heightCm = 168m,
            targetWeightKg = 66m,
            notificationsEnabled = true,
            notificationIntensity = 2,
            onboardingCompleted = true
        });

    private static async Task EnsureProfileAndPlanAsync(HttpClient client, int[] trainingDays)
    {
        await CreateFitnessProfileAsync(client, trainingDays);
        await client.PostAsync("/customers/me/training-plan/generate", null);
    }

    private static async Task<Guid> GetFirstTrainingDayIdAsync(HttpClient client)
    {
        var response = await client.GetAsync("/customers/me/training-plan");
        var plan = await response.Content.ReadFromJsonAsync<TrainingPlanItem>();
        return plan!.Phases.SelectMany(x => x.Weeks).SelectMany(x => x.Days).First().Id;
    }

    private sealed record ActivityHistoryItem(string Title);
    private sealed record ActivitySummaryItem(int DaysTrained, int CaloriesEstimated, List<ActivityByDayItem> ActivityByDay);
    private sealed record ActivityByDayItem(string Day, int ActivityCount);
    private sealed record TrainingPlanItem(List<TrainingPhaseItem> Phases);
    private sealed record TrainingPhaseItem(List<TrainingWeekItem> Weeks);
    private sealed record TrainingWeekItem(List<TrainingDayItem> Days);
    private sealed record TrainingDayItem(Guid Id);
}
