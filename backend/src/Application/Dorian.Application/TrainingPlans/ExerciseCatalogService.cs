namespace Dorian.Application.TrainingPlans;

using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class ExerciseCatalogService : IExerciseCatalogService
{
    private readonly IDorianDbContext _dbContext;

    public ExerciseCatalogService(IDorianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ExerciseCatalogResponse>> ListAsync(string? muscleGroup, CancellationToken cancellationToken)
    {
        var query = _dbContext.ExerciseCatalog.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            if (!Enum.TryParse<ExerciseMuscleGroup>(muscleGroup, true, out var parsed))
            {
                throw new ExerciseCatalogValidationException("Muscle group filter is invalid.");
            }

            query = query.Where(x => x.MuscleGroup == parsed);
        }

        var items = await query.OrderBy(x => x.MuscleGroup).ThenBy(x => x.Name).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<ExerciseCatalogResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _dbContext.ExerciseCatalog.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Exercise not found.");
        return Map(exercise);
    }

    private static ExerciseCatalogResponse Map(ExerciseCatalog exercise) => new(
        exercise.Id,
        exercise.Name,
        exercise.Slug,
        exercise.MuscleGroup,
        exercise.Equipment,
        exercise.Description,
        exercise.Difficulty,
        exercise.VideoUrl,
        exercise.ImageUrl);

    private sealed class ExerciseCatalogValidationException : AppException
    {
        public ExerciseCatalogValidationException(string message) : base(message)
        {
        }
    }
}
