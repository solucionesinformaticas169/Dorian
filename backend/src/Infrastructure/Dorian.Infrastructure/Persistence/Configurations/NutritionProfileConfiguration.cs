namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class NutritionProfileConfiguration : IEntityTypeConfiguration<NutritionProfile>
{
    public void Configure(EntityTypeBuilder<NutritionProfile> builder)
    {
        builder.ToTable("nutrition_profiles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.Goal).HasConversion<int>().IsRequired();
        builder.Property(x => x.DailyCaloriesTarget).IsRequired();
        builder.Property(x => x.ProteinGrams).IsRequired();
        builder.Property(x => x.CarbsGrams).IsRequired();
        builder.Property(x => x.FatGrams).IsRequired();
        builder.Property(x => x.MealsPerDay).IsRequired();
        builder.Property(x => x.WaterLitersTarget).HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(x => x.DietaryRestrictions).HasMaxLength(1000);

        builder.HasIndex(x => x.CustomerId).IsUnique();
    }
}
