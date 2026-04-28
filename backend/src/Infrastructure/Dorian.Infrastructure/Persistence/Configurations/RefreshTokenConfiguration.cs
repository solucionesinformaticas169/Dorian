namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedByIp).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ReplacedByTokenHash).HasMaxLength(200);
        builder.HasIndex(x => x.TokenHash).IsUnique();
    }
}
