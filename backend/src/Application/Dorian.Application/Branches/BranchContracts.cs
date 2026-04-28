namespace Dorian.Application.Branches;

public sealed record BranchResponse(Guid Id, string Code, string Name, string City, string? Address, string? PhoneNumber, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc);
public sealed record CreateBranchRequest(string Code, string Name, string City, string? Address, string? PhoneNumber);
public sealed record UpdateBranchRequest(string Code, string Name, string City, string? Address, string? PhoneNumber, bool IsActive);
