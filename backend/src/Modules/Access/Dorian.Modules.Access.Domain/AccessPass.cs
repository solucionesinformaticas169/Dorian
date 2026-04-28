namespace Dorian.Modules.Access.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class AccessPass : AuditableEntity<Guid>
{
    private AccessPass() : base(Guid.Empty)
    {
    }

    public AccessPass(Guid id, Guid customerId, string qrCodeValue, DateTimeOffset expiresAt) : base(id)
    {
        CustomerId = customerId;
        Regenerate(qrCodeValue, expiresAt);
    }

    public Guid CustomerId { get; private set; }
    public string QrCodeValue { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public AccessPassStatus Status { get; private set; }

    public void Regenerate(string qrCodeValue, DateTimeOffset expiresAt)
    {
        QrCodeValue = qrCodeValue.Trim();
        ExpiresAt = expiresAt;
        Status = expiresAt <= DateTimeOffset.UtcNow ? AccessPassStatus.Expired : AccessPassStatus.Active;
    }

    public void Revoke()
    {
        Status = AccessPassStatus.Revoked;
    }

    public void MarkExpired()
    {
        Status = AccessPassStatus.Expired;
    }
}
