namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class WorkoutActivityConfiguration : IEntityTypeConfiguration<WorkoutActivity>
{
    public void Configure(EntityTypeBuilder<WorkoutActivity> builder)
    {
        builder.ToTable("workout_activities");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.TrainingDayId);
        builder.Property(x => x.CompletedAt).IsRequired();
        builder.Property(x => x.DurationSeconds).IsRequired();
        builder.Property(x => x.CaloriesEstimated).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.CompletedAt);
        builder.HasIndex(x => x.TrainingDayId);

        builder.HasMany(x => x.ExerciseLogs)
            .WithOne(x => x.WorkoutActivity)
            .HasForeignKey(x => x.WorkoutActivityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
