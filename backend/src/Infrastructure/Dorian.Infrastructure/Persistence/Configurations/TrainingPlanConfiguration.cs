namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TrainingPlanConfiguration : IEntityTypeConfiguration<TrainingPlan>
{
    public void Configure(EntityTypeBuilder<TrainingPlan> builder)
    {
        builder.ToTable("training_plans");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CustomerId, x.Status });

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Phases)
            .WithOne(x => x.TrainingPlan)
            .HasForeignKey(x => x.TrainingPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
