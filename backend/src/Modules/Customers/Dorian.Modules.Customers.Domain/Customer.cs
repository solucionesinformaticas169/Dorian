namespace Dorian.Modules.Customers.Domain.Entities;

using Dorian.Modules.Identity.Domain.Entities;
using Dorian.Modules.Memberships.Domain.Entities;
using Dorian.SharedKernel.Primitives;

public sealed class Customer : AuditableEntity<Guid>
{
    private Customer() : base(Guid.Empty)
    {
    }

    public Customer(
        Guid id,
        Guid userId,
        Guid branchId,
        string firstName,
        string lastName,
        string identificationNumber,
        string? phone,
        DateOnly? birthDate,
        Gender gender,
        string? emergencyContactName,
        string? emergencyContactPhone,
        Guid? activeMembershipId,
        CustomerStatus status) : this(
            id,
            userId,
            branchId,
            firstName,
            lastName,
            identificationNumber,
            phone,
            birthDate,
            gender,
            emergencyContactName,
            emergencyContactPhone,
            activeMembershipId,
            null,
            null,
            status)
    {
    }

    public Customer(
        Guid id,
        Guid userId,
        Guid branchId,
        string firstName,
        string lastName,
        string identificationNumber,
        string? phone,
        DateOnly? birthDate,
        Gender gender,
        string? emergencyContactName,
        string? emergencyContactPhone,
        Guid? activeMembershipId,
        DateTimeOffset? activeMembershipStartsAtUtc,
        DateTimeOffset? activeMembershipEndsAtUtc,
        CustomerStatus status) : base(id)
    {
        UserId = userId;
        Update(branchId, firstName, lastName, identificationNumber, phone, birthDate, gender, emergencyContactName, emergencyContactPhone, activeMembershipId, activeMembershipStartsAtUtc, activeMembershipEndsAtUtc, status);
    }

    public Guid UserId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? ActiveMembershipId { get; private set; }
    public DateTimeOffset? ActiveMembershipStartsAtUtc { get; private set; }
    public DateTimeOffset? ActiveMembershipEndsAtUtc { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string IdentificationNumber { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public CustomerStatus Status { get; private set; }
    public User User { get; private set; } = null!;
    public Membership? ActiveMembership { get; private set; }
    public CustomerFitnessProfile? FitnessProfile { get; private set; }
    public ICollection<BodyMeasurement> BodyMeasurements { get; private set; } = [];
    public ICollection<BodyProgressPhoto> BodyProgressPhotos { get; private set; } = [];

    public void Update(
        Guid branchId,
        string firstName,
        string lastName,
        string identificationNumber,
        string? phone,
        DateOnly? birthDate,
        Gender gender,
        string? emergencyContactName,
        string? emergencyContactPhone,
        Guid? activeMembershipId,
        DateTimeOffset? activeMembershipStartsAtUtc,
        DateTimeOffset? activeMembershipEndsAtUtc,
        CustomerStatus status)
    {
        BranchId = branchId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        IdentificationNumber = identificationNumber.Trim().ToUpperInvariant();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        BirthDate = birthDate;
        Gender = gender;
        EmergencyContactName = string.IsNullOrWhiteSpace(emergencyContactName) ? null : emergencyContactName.Trim();
        EmergencyContactPhone = string.IsNullOrWhiteSpace(emergencyContactPhone) ? null : emergencyContactPhone.Trim();
        ActiveMembershipId = activeMembershipId;
        ActiveMembershipStartsAtUtc = activeMembershipId.HasValue ? activeMembershipStartsAtUtc : null;
        ActiveMembershipEndsAtUtc = activeMembershipId.HasValue ? activeMembershipEndsAtUtc : null;
        Status = status;
    }

    public bool HasActiveMembership(DateTimeOffset currentTimeUtc)
    {
        return ActiveMembershipId.HasValue
            && ActiveMembershipStartsAtUtc.HasValue
            && ActiveMembershipEndsAtUtc.HasValue
            && ActiveMembershipStartsAtUtc.Value <= currentTimeUtc
            && ActiveMembershipEndsAtUtc.Value >= currentTimeUtc;
    }
}
