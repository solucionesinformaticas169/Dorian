namespace Dorian.Application.WorkoutActivities;

public interface IWorkoutActivityService
{
    Task<ActivitySummaryResponse> GetMySummaryAsync(int rangeInDays, CancellationToken cancellationToken);
    Task<ActivitySummaryResponse> GetSummaryByCustomerIdAsync(Guid customerId, int rangeInDays, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ActivityHistoryItemResponse>> GetMyHistoryAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MuscleActivityResponse>> GetMyMuscleActivityAsync(CancellationToken cancellationToken);
    Task<WorkoutActivityResponse> CreateManualActivityAsync(CreateWorkoutActivityRequest request, CancellationToken cancellationToken);
    Task EnsureAutomaticActivityForTrainingDayAsync(Guid trainingDayId, CancellationToken cancellationToken);
    Task RemoveAutomaticActivityForTrainingDayAsync(Guid trainingDayId, CancellationToken cancellationToken);
}
