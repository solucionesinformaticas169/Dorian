namespace Dorian.Modules.Nutrition.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class NutritionPlan : AuditableEntity<Guid>
{
    public NutritionPlan(Guid id, Guid clientUserId, string title) : base(id)
    {
        ClientUserId = clientUserId;
        Title = title;
    }

    public Guid ClientUserId { get; private set; }

    public Guid? CoachUserId { get; private set; }

    public string Title { get; private set; }

    public string? Notes { get; private set; }

    public DateOnly? EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }
}
