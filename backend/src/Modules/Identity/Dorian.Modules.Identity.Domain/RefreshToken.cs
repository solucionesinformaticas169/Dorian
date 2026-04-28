namespace Dorian.Modules.Identity.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class RefreshToken : AuditableEntity<Guid>
{
    private RefreshToken() : base(Guid.Empty)
    {
    }

    public RefreshToken(Guid id, Guid userId, string tokenHash, DateTimeOffset expiresAtUtc, string createdByIp) : base(id)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedByIp = createdByIp;
    }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public string CreatedByIp { get; private set; } = string.Empty;

    public string? ReplacedByTokenHash { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTimeOffset.UtcNow;

    public void Revoke(string? replacedByTokenHash = null)
    {
        RevokedAtUtc = DateTimeOffset.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
