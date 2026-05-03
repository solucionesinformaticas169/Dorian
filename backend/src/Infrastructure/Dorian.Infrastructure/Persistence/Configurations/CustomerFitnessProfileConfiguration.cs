namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CustomerFitnessProfileConfiguration : IEntityTypeConfiguration<CustomerFitnessProfile>
{
    public void Configure(EntityTypeBuilder<CustomerFitnessProfile> builder)
    {
        builder.ToTable("customer_fitness_profiles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TrainingDaysSerialized).HasMaxLength(120).IsRequired();
        builder.Property(x => x.PreferredTrainingTime).HasMaxLength(16);
        builder.Property(x => x.WeightKg).HasPrecision(6, 2);
        builder.Property(x => x.HeightCm).HasPrecision(6, 2);
        builder.Property(x => x.TargetWeightKg).HasPrecision(6, 2);
        builder.HasIndex(x => x.CustomerId).IsUnique();

        builder.HasOne(x => x.Customer)
            .WithOne(x => x.FitnessProfile)
            .HasForeignKey<CustomerFitnessProfile>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
