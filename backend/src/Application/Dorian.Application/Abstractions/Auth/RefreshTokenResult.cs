namespace Dorian.Application.Abstractions.Auth;

public sealed record RefreshTokenResult(string Token, DateTimeOffset ExpiresAtUtc);
