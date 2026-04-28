namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AccessPassConfiguration : IEntityTypeConfiguration<AccessPass>
{
    public void Configure(EntityTypeBuilder<AccessPass> builder)
    {
        builder.ToTable("access_passes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.QrCodeValue).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.CustomerId).IsUnique();
        builder.HasIndex(x => x.QrCodeValue).IsUnique();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
