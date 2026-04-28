namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasData(
            new Role(SeedData.CustomerRoleId, RoleNames.Customer, "Gym customer"),
            new Role(SeedData.TrainerRoleId, RoleNames.Trainer, "Gym trainer"),
            new Role(SeedData.ReceptionRoleId, RoleNames.Reception, "Front desk staff"),
            new Role(SeedData.BranchAdminRoleId, RoleNames.BranchAdmin, "Branch administrator"),
            new Role(SeedData.SuperAdminRoleId, RoleNames.SuperAdmin, "Platform super administrator"));
    }
}
