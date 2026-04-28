namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.ToTable("check_ins");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RejectionReason).HasMaxLength(250);
        builder.HasIndex(x => new { x.BranchId, x.CheckedInAt });
        builder.HasIndex(x => new { x.CustomerId, x.CheckedInAt });

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CheckedInByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
