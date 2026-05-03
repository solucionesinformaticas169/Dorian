namespace Dorian.Application.Nutrition;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Nutrition.Domain.Entities;

public sealed record UpdateNutritionProfileRequest(
    int MealsPerDay,
    string? DietaryRestrictions);

public sealed record NutritionProfileResponse(
    Guid Id,
    Guid CustomerId,
    FitnessGoal Goal,
    int DailyCaloriesTarget,
    int ProteinGrams,
    int CarbsGrams,
    int FatGrams,
    int MealsPerDay,
    decimal WaterLitersTarget,
    string? DietaryRestrictions,
    string Disclaimer,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record MealPlanResponse(
    Guid Id,
    Guid CustomerId,
    string Title,
    string Description,
    TrainingDay? DayOfWeek,
    IReadOnlyCollection<MealItemResponse> Items,
    DateTimeOffset CreatedAtUtc);

public sealed record MealItemResponse(
    Guid Id,
    MealType MealType,
    string Name,
    string Description,
    int Calories,
    int ProteinGrams,
    int CarbsGrams,
    int FatGrams);
