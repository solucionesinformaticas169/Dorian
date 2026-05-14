namespace Dorian.Application.Staff;

public sealed record StaffMemberResponse(
    Guid Id,
    string Email,
    string FullName,
    string? PhoneNumber,
    Guid? BranchId,
    string? BranchName,
    IReadOnlyCollection<string> Roles,
    string PrimaryRole,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CreateStaffMemberRequest(
    string Email,
    string Password,
    string FullName,
    string? PhoneNumber,
    Guid? BranchId,
    string Role);

public sealed record UpdateStaffMemberRequest(
    string FullName,
    string? PhoneNumber,
    Guid? BranchId,
    string Role,
    bool IsActive,
    string? Password);
