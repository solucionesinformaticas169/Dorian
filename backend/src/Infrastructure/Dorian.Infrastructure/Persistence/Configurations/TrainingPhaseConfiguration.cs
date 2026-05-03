namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TrainingPhaseConfiguration : IEntityTypeConfiguration<TrainingPhase>
{
    public void Configure(EntityTypeBuilder<TrainingPhase> builder)
    {
        builder.ToTable("training_phases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(600).IsRequired();
        builder.HasIndex(x => new { x.TrainingPlanId, x.Order }).IsUnique();

        builder.HasMany(x => x.Weeks)
            .WithOne(x => x.TrainingPhase)
            .HasForeignKey(x => x.TrainingPhaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
