namespace Dorian.Application.Branches;

public sealed record BranchResponse(Guid Id, string Code, string Name, string City, string? Address, string? PhoneNumber, string? OpeningHours, string? MapUrl, decimal? Latitude, decimal? Longitude, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc);
public sealed record CreateBranchRequest(string Code, string Name, string City, string? Address, string? PhoneNumber, string? OpeningHours, string? MapUrl, decimal? Latitude, decimal? Longitude);
public sealed record UpdateBranchRequest(string Code, string Name, string City, string? Address, string? PhoneNumber, string? OpeningHours, string? MapUrl, decimal? Latitude, decimal? Longitude, bool IsActive);
