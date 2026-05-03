namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
    {
        builder.ToTable("body_measurements");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WeightKg).HasPrecision(8, 2).IsRequired();
        builder.Property(x => x.HeightCm).HasPrecision(8, 2).IsRequired();
        builder.Property(x => x.BodyFatPercentage).HasPrecision(5, 2);
        builder.Property(x => x.MuscleMassKg).HasPrecision(8, 2);
        builder.Property(x => x.BoneMassKg).HasPrecision(8, 2);
        builder.Property(x => x.ResidualMassKg).HasPrecision(8, 2);
        builder.Property(x => x.WaistCm).HasPrecision(8, 2);
        builder.Property(x => x.ChestCm).HasPrecision(8, 2);
        builder.Property(x => x.HipCm).HasPrecision(8, 2);
        builder.Property(x => x.ShouldersCm).HasPrecision(8, 2);
        builder.Property(x => x.LeftArmCm).HasPrecision(8, 2);
        builder.Property(x => x.RightArmCm).HasPrecision(8, 2);
        builder.Property(x => x.LeftLegCm).HasPrecision(8, 2);
        builder.Property(x => x.RightLegCm).HasPrecision(8, 2);
        builder.Property(x => x.LeftCalfCm).HasPrecision(8, 2);
        builder.Property(x => x.RightCalfCm).HasPrecision(8, 2);
        builder.Property(x => x.NeckCm).HasPrecision(8, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => new { x.CustomerId, x.MeasuredAt });

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.BodyMeasurements)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
