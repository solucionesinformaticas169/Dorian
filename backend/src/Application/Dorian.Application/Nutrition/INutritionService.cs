namespace Dorian.Application.Nutrition;

public interface INutritionService
{
    Task<NutritionProfileResponse?> GetMyProfileAsync(CancellationToken cancellationToken);
    Task<NutritionProfileResponse?> GetProfileByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<NutritionProfileResponse> GenerateMyProfileAsync(CancellationToken cancellationToken);
    Task<NutritionProfileResponse> UpdateMyProfileAsync(UpdateNutritionProfileRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MealPlanResponse>> GetMyMealPlanAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MealPlanResponse>> GetMealPlanByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MealPlanResponse>> GenerateMyMealPlanAsync(CancellationToken cancellationToken);
}
