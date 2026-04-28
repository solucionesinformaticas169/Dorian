namespace Dorian.Application.Abstractions.Auth;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);
