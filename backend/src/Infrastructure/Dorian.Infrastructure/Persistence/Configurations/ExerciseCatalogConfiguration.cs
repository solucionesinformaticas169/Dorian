namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Infrastructure.Persistence;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ExerciseCatalogConfiguration : IEntityTypeConfiguration<ExerciseCatalog>
{
    public void Configure(EntityTypeBuilder<ExerciseCatalog> builder)
    {
        builder.ToTable("exercise_catalog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(600).IsRequired();
        builder.Property(x => x.VideoUrl).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.MuscleGroup);

        builder.HasData(ExerciseCatalogSeed.Items.Select(item => new
        {
            Id = Guid.Parse(item.Id),
            item.Name,
            item.Slug,
            item.MuscleGroup,
            item.Equipment,
            item.Description,
            item.Difficulty,
            item.VideoUrl,
            item.ImageUrl,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = (DateTimeOffset?)null
        }));
    }
}
