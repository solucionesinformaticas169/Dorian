namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

public sealed class TrainingPlanEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TrainingPlanEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Generates_Beginner_Training_Plan()
    {
        var client = await CreateAuthenticatedClientAsync("nomembership@dorian.test");
        await CreateFitnessProfileAsync(client, goal: 1, experienceLevel: 1, focusMuscleGroup: 1, trainingDays: [1, 3, 5]);

        var response = await client.PostAsync("/customers/me/training-plan/generate", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<TrainingPlanItem>();
        payload.Should().NotBeNull();
        payload!.Goal.Should().Be(1);
        payload.ExperienceLevel.Should().Be(1);
        payload.Phases.Should().Contain(x => x.Name == 1);
        payload.Phases.SelectMany(x => x.Weeks).SelectMany(x => x.Days).Should().OnlyContain(day => day.Exercises.Count >= 3);
    }

    [Fact]
    public async Task Generates_Hypertrophy_Training_Plan()
    {
        var client = await CreateAuthenticatedClientAsync("expiredmembership@dorian.test");
        await CreateFitnessProfileAsync(client, goal: 3, experienceLevel: 2, focusMuscleGroup: 2, trainingDays: [1, 2, 4, 6]);

        var response = await client.PostAsync("/customers/me/training-plan/generate", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<TrainingPlanItem>();
        payload.Should().NotBeNull();
        payload!.Phases.Should().Contain(x => x.Name == 3);
        payload.FocusMuscleGroup.Should().Be(2);
    }

    [Fact]
    public async Task Generates_Plan_Using_Available_Days()
    {
        var client = await CreateAuthenticatedClientAsync("othercustomer@dorian.test");
        await CreateFitnessProfileAsync(client, goal: 4, experienceLevel: 2, focusMuscleGroup: 3, trainingDays: [2, 5]);

        var response = await client.PostAsync("/customers/me/training-plan/generate", null);
        var payload = await response.Content.ReadFromJsonAsync<TrainingPlanItem>();

        payload.Should().NotBeNull();
        payload!.Phases.SelectMany(x => x.Weeks).All(week => week.Days.Select(day => day.DayOfWeek).Order().SequenceEqual(new[] { 2, 5 })).Should().BeTrue();
    }

    [Fact]
    public async Task Can_Mark_Training_Day_As_Completed()
    {
        var client = await CreateAuthenticatedClientAsync("revokedpass@dorian.test");
        await CreateFitnessProfileAsync(client, goal: 2, experienceLevel: 2, focusMuscleGroup: 6, trainingDays: [1, 4, 6]);
        var generated = await client.PostAsync("/customers/me/training-plan/generate", null);
        var plan = await generated.Content.ReadFromJsonAsync<TrainingPlanItem>();
        var dayId = plan!.Phases.SelectMany(x => x.Weeks).SelectMany(x => x.Days).First().Id;

        var response = await client.PutAsync($"/training-days/{dayId}/complete", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<TrainingDayItem>();
        payload.Should().NotBeNull();
        payload!.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Customer_Cannot_Access_Plan_Of_Other_Customer()
    {
        var customerClient = await CreateAuthenticatedClientAsync(_factory.CustomerEmail);
        await CreateFitnessProfileAsync(customerClient, goal: 3, experienceLevel: 2, focusMuscleGroup: 1, trainingDays: [1, 3, 5]);
        await customerClient.PostAsync("/customers/me/training-plan/generate", null);

        var client = await CreateAuthenticatedClientAsync(_factory.MainBranchSecondCustomerEmail);
        var response = await client.GetAsync($"/customers/{_factory.SeededCustomerId}/training-plan");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static Task<HttpResponseMessage> CreateFitnessProfileAsync(HttpClient client, int goal, int experienceLevel, int focusMuscleGroup, int[] trainingDays)
        => client.PostAsJsonAsync("/customers/me/fitness-profile", new
        {
            goal,
            focusMuscleGroup,
            experienceLevel,
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

    private sealed record TrainingPlanItem(int Goal, int ExperienceLevel, int FocusMuscleGroup, List<TrainingPhaseItem> Phases);
    private sealed record TrainingPhaseItem(int Name, List<TrainingWeekItem> Weeks);
    private sealed record TrainingWeekItem(List<TrainingDayItem> Days);
    private sealed record TrainingDayItem(Guid Id, int DayOfWeek, DateTimeOffset? CompletedAt, List<TrainingExerciseItem> Exercises);
    private sealed record TrainingExerciseItem(string Name);
}
