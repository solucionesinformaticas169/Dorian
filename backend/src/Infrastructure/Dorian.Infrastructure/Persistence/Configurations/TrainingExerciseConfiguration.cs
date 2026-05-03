namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TrainingExerciseConfiguration : IEntityTypeConfiguration<TrainingExercise>
{
    public void Configure(EntityTypeBuilder<TrainingExercise> builder)
    {
        builder.ToTable("training_exercises");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Reps).HasMaxLength(60).IsRequired();
        builder.Property(x => x.WeightKg).HasPrecision(8, 2);
        builder.Property(x => x.Notes).HasMaxLength(400);
        builder.HasIndex(x => new { x.TrainingDayId, x.Order }).IsUnique();

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
