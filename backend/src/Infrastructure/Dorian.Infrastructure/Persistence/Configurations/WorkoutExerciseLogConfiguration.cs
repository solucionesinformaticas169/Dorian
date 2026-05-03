namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class WorkoutExerciseLogConfiguration : IEntityTypeConfiguration<WorkoutExerciseLog>
{
    public void Configure(EntityTypeBuilder<WorkoutExerciseLog> builder)
    {
        builder.ToTable("workout_exercise_logs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkoutActivityId).IsRequired();
        builder.Property(x => x.ExerciseName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.MuscleGroup).HasConversion<int>().IsRequired();
        builder.Property(x => x.Sets).IsRequired();
        builder.Property(x => x.Reps).HasMaxLength(80).IsRequired();
        builder.Property(x => x.WeightKg).HasColumnType("numeric(10,2)");
        builder.Property(x => x.Completed).IsRequired();

        builder.HasIndex(x => x.WorkoutActivityId);
        builder.HasIndex(x => x.MuscleGroup);
    }
}
