namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.IdentificationNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.EmergencyContactName).HasMaxLength(100);
        builder.Property(x => x.EmergencyContactPhone).HasMaxLength(30);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.IdentificationNumber).IsUnique();

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Customer>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ActiveMembership)
            .WithMany()
            .HasForeignKey(x => x.ActiveMembershipId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.FitnessProfile)
            .WithOne(x => x.Customer)
            .HasForeignKey<CustomerFitnessProfile>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BodyMeasurements)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BodyProgressPhotos)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
