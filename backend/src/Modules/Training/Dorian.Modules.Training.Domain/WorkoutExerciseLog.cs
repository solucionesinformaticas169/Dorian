namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class WorkoutExerciseLog : AuditableEntity<Guid>
{
    private WorkoutExerciseLog() : base(Guid.Empty)
    {
    }

    public WorkoutExerciseLog(
        Guid id,
        Guid workoutActivityId,
        string exerciseName,
        ExerciseMuscleGroup muscleGroup,
        int sets,
        string reps,
        decimal? weightKg,
        bool completed) : base(id)
    {
        WorkoutActivityId = workoutActivityId;
        ExerciseName = exerciseName.Trim();
        MuscleGroup = muscleGroup;
        Sets = sets;
        Reps = reps.Trim();
        WeightKg = weightKg;
        Completed = completed;
    }

    public Guid WorkoutActivityId { get; private set; }
    public string ExerciseName { get; private set; } = string.Empty;
    public ExerciseMuscleGroup MuscleGroup { get; private set; }
    public int Sets { get; private set; }
    public string Reps { get; private set; } = string.Empty;
    public decimal? WeightKg { get; private set; }
    public bool Completed { get; private set; }
    public WorkoutActivity WorkoutActivity { get; private set; } = null!;
}
