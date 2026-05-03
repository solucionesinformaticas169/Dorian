namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TrainingPlanDayConfiguration : IEntityTypeConfiguration<TrainingPlanDay>
{
    public void Configure(EntityTypeBuilder<TrainingPlanDay> builder)
    {
        builder.ToTable("training_days");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(160).IsRequired();
        builder.HasIndex(x => new { x.TrainingWeekId, x.DayOfWeek }).IsUnique();

        builder.HasMany(x => x.Exercises)
            .WithOne(x => x.TrainingDay)
            .HasForeignKey(x => x.TrainingDayId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.WorkoutActivities)
            .WithOne(x => x.TrainingDay)
            .HasForeignKey(x => x.TrainingDayId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
