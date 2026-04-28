namespace Dorian.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "Dorian";
    public string Audience { get; init; } = "Dorian.Clients";
    public string SecretKey { get; init; } = "super-secret-development-key-change-me";
    public int AccessTokenMinutes { get; init; } = 60;
    public int RefreshTokenDays { get; init; } = 7;
}
