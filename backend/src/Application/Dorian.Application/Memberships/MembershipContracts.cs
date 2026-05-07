namespace Dorian.Application.Memberships;

public sealed record MembershipResponse(Guid Id, Guid? BranchId, string Name, int DurationInDays, decimal Price, string Currency, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc);
public sealed record CreateMembershipRequest(Guid? BranchId, string Name, int DurationInDays, decimal Price, string Currency, bool IsActive);
public sealed record UpdateMembershipRequest(Guid? BranchId, string Name, int DurationInDays, decimal Price, string Currency, bool IsActive);
