namespace Dorian.Modules.Nutrition.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class MealItem : AuditableEntity<Guid>
{
    private MealItem() : base(Guid.Empty)
    {
    }

    public MealItem(
        Guid id,
        Guid mealPlanId,
        MealType mealType,
        string name,
        string description,
        int calories,
        int proteinGrams,
        int carbsGrams,
        int fatGrams) : base(id)
    {
        MealPlanId = mealPlanId;
        MealType = mealType;
        Name = name.Trim();
        Description = description.Trim();
        Calories = calories;
        ProteinGrams = proteinGrams;
        CarbsGrams = carbsGrams;
        FatGrams = fatGrams;
    }

    public Guid MealPlanId { get; private set; }
    public MealType MealType { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Calories { get; private set; }
    public int ProteinGrams { get; private set; }
    public int CarbsGrams { get; private set; }
    public int FatGrams { get; private set; }
    public MealPlan MealPlan { get; private set; } = null!;
}
