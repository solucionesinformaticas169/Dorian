namespace Dorian.Modules.Nutrition.Domain.Entities;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.SharedKernel.Primitives;

public sealed class NutritionProfile : AuditableEntity<Guid>
{
    private NutritionProfile() : base(Guid.Empty)
    {
    }

    public NutritionProfile(
        Guid id,
        Guid customerId,
        FitnessGoal goal,
        int dailyCaloriesTarget,
        int proteinGrams,
        int carbsGrams,
        int fatGrams,
        int mealsPerDay,
        decimal waterLitersTarget,
        string? dietaryRestrictions) : base(id)
    {
        CustomerId = customerId;
        Update(goal, dailyCaloriesTarget, proteinGrams, carbsGrams, fatGrams, mealsPerDay, waterLitersTarget, dietaryRestrictions);
    }

    public Guid CustomerId { get; private set; }
    public FitnessGoal Goal { get; private set; }
    public int DailyCaloriesTarget { get; private set; }
    public int ProteinGrams { get; private set; }
    public int CarbsGrams { get; private set; }
    public int FatGrams { get; private set; }
    public int MealsPerDay { get; private set; }
    public decimal WaterLitersTarget { get; private set; }
    public string? DietaryRestrictions { get; private set; }

    public void Update(
        FitnessGoal goal,
        int dailyCaloriesTarget,
        int proteinGrams,
        int carbsGrams,
        int fatGrams,
        int mealsPerDay,
        decimal waterLitersTarget,
        string? dietaryRestrictions)
    {
        Goal = goal;
        DailyCaloriesTarget = dailyCaloriesTarget;
        ProteinGrams = proteinGrams;
        CarbsGrams = carbsGrams;
        FatGrams = fatGrams;
        MealsPerDay = mealsPerDay;
        WaterLitersTarget = decimal.Round(waterLitersTarget, 2, MidpointRounding.AwayFromZero);
        DietaryRestrictions = string.IsNullOrWhiteSpace(dietaryRestrictions) ? null : dietaryRestrictions.Trim();
    }
}
