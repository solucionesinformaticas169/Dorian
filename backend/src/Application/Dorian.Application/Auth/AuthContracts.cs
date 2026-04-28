namespace Dorian.Application.Auth;

public sealed record RegisterRequest(string Email, string Password, string FullName, string? PhoneNumber, Guid? BranchId);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record LogoutRequest(string RefreshToken);
public sealed record AuthenticatedUserResponse(Guid Id, string Email, string FullName, Guid? BranchId, IReadOnlyCollection<string> Roles);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAtUtc, DateTimeOffset RefreshTokenExpiresAtUtc, AuthenticatedUserResponse User);
