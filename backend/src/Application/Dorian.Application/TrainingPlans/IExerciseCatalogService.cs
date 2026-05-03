namespace Dorian.Application.TrainingPlans;

public interface IExerciseCatalogService
{
    Task<IReadOnlyCollection<ExerciseCatalogResponse>> ListAsync(string? muscleGroup, CancellationToken cancellationToken);
    Task<ExerciseCatalogResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
