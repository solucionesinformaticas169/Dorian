namespace Dorian.Application.TrainingPlans;

public interface ITrainingPlanService
{
    Task<TrainingPlanResponse?> GetMyPlanAsync(CancellationToken cancellationToken);
    Task<TrainingPlanResponse?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<TrainingPlanResponse> GenerateForCurrentCustomerAsync(CancellationToken cancellationToken);
    Task<TrainingPlanResponse> GenerateForCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    Task<TrainingPlanDayResponse> CompleteDayAsync(Guid trainingDayId, CancellationToken cancellationToken);
    Task<TrainingPlanDayResponse> UncompleteDayAsync(Guid trainingDayId, CancellationToken cancellationToken);
}
