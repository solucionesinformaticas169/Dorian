namespace Dorian.Application.Customers;

using Dorian.Modules.Customers.Domain.Entities;

public sealed record CustomerResponse(
    Guid Id,
    Guid UserId,
    string Email,
    Guid? BranchId,
    Guid? ActiveMembershipId,
    string? ActiveMembershipName,
    int? ActiveMembershipDurationInDays,
    decimal? ActiveMembershipPrice,
    string? ActiveMembershipCurrency,
    DateTimeOffset? ActiveMembershipStartsAtUtc,
    DateTimeOffset? ActiveMembershipEndsAtUtc,
    string FirstName,
    string LastName,
    string IdentificationNumber,
    string? Phone,
    DateOnly? BirthDate,
    Gender Gender,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    CustomerStatus Status,
    bool OnboardingCompleted,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CustomerSummaryResponse(
    int TotalCustomers,
    int ActiveCustomers,
    int InactiveCustomers);

public sealed record CreateCustomerRequest(
    string Email,
    string Password,
    Guid? BranchId,
    Guid? ActiveMembershipId,
    DateTimeOffset? ActiveMembershipStartsAtUtc,
    DateTimeOffset? ActiveMembershipEndsAtUtc,
    string FirstName,
    string LastName,
    string IdentificationNumber,
    string? Phone,
    DateOnly? BirthDate,
    Gender Gender,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    CustomerStatus Status);

public sealed record UpdateCustomerRequest(
    Guid? BranchId,
    Guid? ActiveMembershipId,
    DateTimeOffset? ActiveMembershipStartsAtUtc,
    DateTimeOffset? ActiveMembershipEndsAtUtc,
    string FirstName,
    string LastName,
    string IdentificationNumber,
    string? Phone,
    DateOnly? BirthDate,
    Gender Gender,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    CustomerStatus Status);
