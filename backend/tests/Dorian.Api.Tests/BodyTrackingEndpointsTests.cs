namespace Dorian.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

public sealed class BodyTrackingEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BodyTrackingEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customer_Can_Create_Body_Measurement()
    {
        var client = await CreateAuthenticatedClientAsync(_factory.MainBranchSecondCustomerEmail);

        var response = await client.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-1), 72m, 168m));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<BodyMeasurementItem>();
        payload.Should().NotBeNull();
        payload!.WeightKg.Should().Be(72m);
        payload.Bmi.Should().Be(25.51m);
    }

    [Fact]
    public async Task Customer_Can_Get_Latest_Body_Measurement()
    {
        var client = await CreateAuthenticatedClientAsync("othercustomer@dorian.test");
        await client.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-5), 74m, 168m));
        await client.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-1), 72m, 168m));

        var response = await client.GetAsync("/customers/me/body-measurements/latest");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<BodyMeasurementItem>();
        payload.Should().NotBeNull();
        payload!.WeightKg.Should().Be(72m);
    }

    [Fact]
    public async Task Customer_Cannot_Access_Body_Measurements_Of_Other_Customer()
    {
        var client = await CreateAuthenticatedClientAsync(_factory.CustomerEmail);

        var response = await client.GetAsync($"/customers/{_factory.SecondaryCustomerId}/body-measurements");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Customer_Can_Delete_Own_Body_Measurement()
    {
        var client = await CreateAuthenticatedClientAsync("nomembership@dorian.test");
        var create = await client.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-1), 72m, 168m));
        var created = await create.Content.ReadFromJsonAsync<BodyMeasurementItem>();

        var delete = await client.DeleteAsync($"/customers/me/body-measurements/{created!.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var list = await client.GetFromJsonAsync<List<BodyMeasurementItem>>("/customers/me/body-measurements");
        list.Should().BeEmpty();
    }

    [Fact]
    public async Task Body_Summary_Returns_Correct_Current_Data()
    {
        var client = await CreateAuthenticatedClientAsync("expiredmembership@dorian.test");
        await client.PostAsJsonAsync("/customers/me/fitness-profile", new
        {
            goal = 3,
            focusMuscleGroup = 1,
            experienceLevel = 2,
            gymType = 2,
            includeCardio = true,
            trainingDays = new[] { 1, 3, 5 },
            preferredTrainingTime = "18:30",
            gender = 2,
            birthDate = new DateOnly(1995, 5, 20),
            weightKg = 73m,
            heightCm = 168m,
            targetWeightKg = 65m,
            notificationsEnabled = true,
            notificationIntensity = 2,
            onboardingCompleted = true
        });
        await client.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-3), 73m, 168m, bodyFatPercentage: 29m, waistCm: 81m));
        await client.PostAsJsonAsync("/customers/me/body-progress-photos", new
        {
            photoUrl = "https://example.com/progress/front.jpg",
            takenAt = DateTimeOffset.UtcNow.AddDays(-2),
            type = 1,
            notes = "Semana 1"
        });

        var response = await client.GetAsync("/customers/me/body-summary");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<BodySummaryItem>();
        payload.Should().NotBeNull();
        payload!.CurrentWeightKg.Should().Be(73m);
        payload.TargetWeightKg.Should().Be(65m);
        payload.HeightCm.Should().Be(168m);
        payload.Bmi.Should().Be(25.86m);
        payload.BmiLabel.Should().Be("Sobrepeso");
        payload.WeightHistory.Should().ContainSingle();
        payload.ProgressPhotos.Should().ContainSingle();
    }

    [Fact]
    public async Task BranchAdmin_Can_View_Body_Summary_Of_Customer_In_Own_Branch()
    {
        var customerClient = await CreateAuthenticatedClientAsync(_factory.CustomerEmail);
        await customerClient.PostAsJsonAsync("/customers/me/body-measurements", CreateMeasurementPayload(DateTimeOffset.UtcNow.AddDays(-1), 71m, 168m));

        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, _factory.BranchAdminEmail, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/customers/{_factory.SeededCustomerId}/body-summary");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(client, email, _factory.Password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static object CreateMeasurementPayload(
        DateTimeOffset measuredAt,
        decimal weightKg,
        decimal heightCm,
        decimal? bodyFatPercentage = null,
        decimal? waistCm = null) => new
    {
        measuredAt,
        weightKg,
        heightCm,
        bodyFatPercentage,
        muscleMassKg = 28m,
        boneMassKg = 3.2m,
        residualMassKg = 6.5m,
        waistCm,
        chestCm = 95m,
        hipCm = 98m,
        shouldersCm = 112m,
        leftArmCm = 31m,
        rightArmCm = 31.5m,
        leftLegCm = 55m,
        rightLegCm = 55m,
        leftCalfCm = 36m,
        rightCalfCm = 36m,
        neckCm = 34m,
        notes = "Seguimiento semanal"
    };

    private sealed record BodyMeasurementItem(Guid Id, decimal WeightKg, decimal Bmi);

    private sealed record BodySummaryItem(
        decimal? CurrentWeightKg,
        decimal? TargetWeightKg,
        decimal? HeightCm,
        decimal? Bmi,
        string BmiLabel,
        List<BodyWeightHistoryItem> WeightHistory,
        List<BodyPhotoItem> ProgressPhotos);

    private sealed record BodyWeightHistoryItem(DateTimeOffset MeasuredAt, decimal WeightKg, decimal Bmi);
    private sealed record BodyPhotoItem(Guid Id, string PhotoUrl);
}
