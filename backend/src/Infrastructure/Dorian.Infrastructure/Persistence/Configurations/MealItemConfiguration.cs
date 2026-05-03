namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MealItemConfiguration : IEntityTypeConfiguration<MealItem>
{
    public void Configure(EntityTypeBuilder<MealItem> builder)
    {
        builder.ToTable("meal_items");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MealPlanId).IsRequired();
        builder.Property(x => x.MealType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Calories).IsRequired();
        builder.Property(x => x.ProteinGrams).IsRequired();
        builder.Property(x => x.CarbsGrams).IsRequired();
        builder.Property(x => x.FatGrams).IsRequired();

        builder.HasIndex(x => x.MealPlanId);
    }
}
