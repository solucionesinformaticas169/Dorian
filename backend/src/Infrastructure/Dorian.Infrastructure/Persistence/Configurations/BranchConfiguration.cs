namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Branches.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.OpeningHours).HasMaxLength(120);
        builder.Property(x => x.MapUrl).HasMaxLength(500);
        builder.Property(x => x.Latitude).HasColumnType("numeric(9,6)");
        builder.Property(x => x.Longitude).HasColumnType("numeric(9,6)");
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasData(
            new Branch(SeedData.ElCebollarBranchId, "CEBOLLAR", "El Cebollar", "Cuenca", "Av. Abelardo J Andrade y Reinaldo Chico Peñaherrera", null, null, SeedData.BranchMaps.ElCebollar, null, null),
            new Branch(SeedData.CementerioBranchId, "CEMENTERIO", "Cementerio", "Cuenca", "Av. Gonzales Suárez", null, null, SeedData.BranchMaps.Cementerio, null, null),
            new Branch(SeedData.ParqueIndustrialBranchId, "PARQUEIND", "Parque Industrial", "Cuenca", "Octavio Chacón Moscoso y Cornelio Vintimilla", null, null, SeedData.BranchMaps.ParqueIndustrial, null, null),
            new Branch(SeedData.ElTiempoBranchId, "ELTIEMPO", "El Tiempo", "Cuenca", "Av. Loja y Rodrigo de Triana", null, null, SeedData.BranchMaps.ElTiempo, null, null),
            new Branch(SeedData.AzoguesBranchId, "AZOGUES", "Azogues", "Azogues", "Calle Simón Bolívar", null, null, SeedData.BranchMaps.Azogues, null, null));
    }
}
