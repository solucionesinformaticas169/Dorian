namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Classes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CustomerId, x.ClassSessionId }).IsUnique();
    }
}
