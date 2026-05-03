namespace Dorian.Modules.Training.Domain.Entities;

using Dorian.Modules.Customers.Domain.Entities;
using Dorian.SharedKernel.Primitives;

public sealed class TrainingPlan : AuditableEntity<Guid>
{
    private TrainingPlan() : base(Guid.Empty)
    {
    }

    public TrainingPlan(
        Guid id,
        Guid customerId,
        FitnessGoal goal,
        FitnessExperienceLevel experienceLevel,
        FocusMuscleGroup focusMuscleGroup,
        DateOnly startDate) : base(id)
    {
        CustomerId = customerId;
        Goal = goal;
        ExperienceLevel = experienceLevel;
        FocusMuscleGroup = focusMuscleGroup;
        Status = TrainingPlanStatus.Active;
        StartDate = startDate;
    }

    public Guid CustomerId { get; private set; }
    public FitnessGoal Goal { get; private set; }
    public FitnessExperienceLevel ExperienceLevel { get; private set; }
    public FocusMuscleGroup FocusMuscleGroup { get; private set; }
    public TrainingPlanStatus Status { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public ICollection<TrainingPhase> Phases { get; private set; } = [];

    public void Cancel(DateOnly endDate)
    {
        Status = TrainingPlanStatus.Cancelled;
        EndDate = endDate;
    }

    public void Complete(DateOnly endDate)
    {
        Status = TrainingPlanStatus.Completed;
        EndDate = endDate;
    }

    public void Pause()
    {
        Status = TrainingPlanStatus.Paused;
    }
}
