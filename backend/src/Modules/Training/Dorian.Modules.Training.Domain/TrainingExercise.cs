namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class TrainingExercise : AuditableEntity<Guid>
{
    private TrainingExercise() : base(Guid.Empty)
    {
    }

    public TrainingExercise(
        Guid id,
        Guid trainingDayId,
        Guid? exerciseId,
        string name,
        ExerciseMuscleGroup muscleGroup,
        int sets,
        string reps,
        int restSeconds,
        decimal? weightKg,
        string? notes,
        int order) : base(id)
    {
        TrainingDayId = trainingDayId;
        ExerciseId = exerciseId;
        Name = name.Trim();
        MuscleGroup = muscleGroup;
        Sets = sets;
        Reps = reps.Trim();
        RestSeconds = restSeconds;
        WeightKg = weightKg;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Order = order;
    }

    public Guid TrainingDayId { get; private set; }
    public Guid? ExerciseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ExerciseMuscleGroup MuscleGroup { get; private set; }
    public int Sets { get; private set; }
    public string Reps { get; private set; } = string.Empty;
    public int RestSeconds { get; private set; }
    public decimal? WeightKg { get; private set; }
    public string? Notes { get; private set; }
    public int Order { get; private set; }
    public TrainingPlanDay TrainingDay { get; private set; } = null!;
    public ExerciseCatalog? Exercise { get; private set; }
}
