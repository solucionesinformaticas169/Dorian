namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TrainingWeekConfiguration : IEntityTypeConfiguration<TrainingWeek>
{
    public void Configure(EntityTypeBuilder<TrainingWeek> builder)
    {
        builder.ToTable("training_weeks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(600).IsRequired();
        builder.HasIndex(x => new { x.TrainingPhaseId, x.WeekNumber }).IsUnique();

        builder.HasMany(x => x.Days)
            .WithOne(x => x.TrainingWeek)
            .HasForeignKey(x => x.TrainingWeekId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
