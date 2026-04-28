namespace Dorian.Modules.Classes.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class ClassSession : AuditableEntity<Guid>
{
    private ClassSession() : base(Guid.Empty)
    {
    }

    public ClassSession(Guid id, Guid branchId, Guid? trainerUserId, string name, string? description, DateTimeOffset startTime, DateTimeOffset endTime, int capacity, ClassSessionStatus status) : base(id)
    {
        Update(branchId, trainerUserId, name, description, startTime, endTime, capacity, status);
    }

    public Guid BranchId { get; private set; }
    public Guid? TrainerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public int Capacity { get; private set; }
    public ClassSessionStatus Status { get; private set; }

    public void Update(Guid branchId, Guid? trainerUserId, string name, string? description, DateTimeOffset startTime, DateTimeOffset endTime, int capacity, ClassSessionStatus status)
    {
        BranchId = branchId;
        TrainerUserId = trainerUserId;
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        StartTime = startTime;
        EndTime = endTime;
        Capacity = capacity;
        Status = status;
    }
}
