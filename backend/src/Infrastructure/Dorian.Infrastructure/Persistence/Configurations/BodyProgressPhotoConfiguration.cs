namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BodyProgressPhotoConfiguration : IEntityTypeConfiguration<BodyProgressPhoto>
{
    public void Configure(EntityTypeBuilder<BodyProgressPhoto> builder)
    {
        builder.ToTable("body_progress_photos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhotoUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => new { x.CustomerId, x.TakenAt });

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.BodyProgressPhotos)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
