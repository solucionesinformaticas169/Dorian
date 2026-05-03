namespace Dorian.Infrastructure.Persistence.Configurations;

using Dorian.Modules.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("meal_plans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.DayOfWeek).HasConversion<int?>();

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.DayOfWeek);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.MealPlan)
            .HasForeignKey(x => x.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
