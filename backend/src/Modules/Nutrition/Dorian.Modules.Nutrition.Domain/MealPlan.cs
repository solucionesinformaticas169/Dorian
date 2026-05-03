namespace Dorian.Modules.Nutrition.Domain.Entities;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.SharedKernel.Primitives;

public sealed class MealPlan : AuditableEntity<Guid>
{
    private MealPlan() : base(Guid.Empty)
    {
    }

    public MealPlan(Guid id, Guid customerId, string title, string description, TrainingDay? dayOfWeek) : base(id)
    {
        CustomerId = customerId;
        Title = title.Trim();
        Description = description.Trim();
        DayOfWeek = dayOfWeek;
    }

    public Guid CustomerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TrainingDay? DayOfWeek { get; private set; }
    public ICollection<MealItem> Items { get; private set; } = [];
}
