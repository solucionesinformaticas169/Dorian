namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class ExerciseCatalog : AuditableEntity<Guid>
{
    private ExerciseCatalog() : base(Guid.Empty)
    {
    }

    public ExerciseCatalog(
        Guid id,
        string name,
        string slug,
        ExerciseMuscleGroup muscleGroup,
        ExerciseEquipment equipment,
        string description,
        ExerciseDifficulty difficulty,
        string? videoUrl,
        string? imageUrl) : base(id)
    {
        Name = name.Trim();
        Slug = slug.Trim();
        MuscleGroup = muscleGroup;
        Equipment = equipment;
        Description = description.Trim();
        Difficulty = difficulty;
        VideoUrl = string.IsNullOrWhiteSpace(videoUrl) ? null : videoUrl.Trim();
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
    }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public ExerciseMuscleGroup MuscleGroup { get; private set; }
    public ExerciseEquipment Equipment { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public ExerciseDifficulty Difficulty { get; private set; }
    public string? VideoUrl { get; private set; }
    public string? ImageUrl { get; private set; }
}
