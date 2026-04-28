namespace Dorian.Modules.Payments.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class Payment : AuditableEntity<Guid>
{
    public Payment(Guid id, Guid clientUserId, decimal amount, string currency, PaymentMethod method) : base(id)
    {
        ClientUserId = clientUserId;
        Amount = amount;
        Currency = currency;
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public Guid ClientUserId { get; private set; }

    public Guid? MembershipId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; }

    public PaymentMethod Method { get; private set; }

    public PaymentStatus Status { get; private set; }

    public string? ExternalProvider { get; private set; }

    public string? ExternalReference { get; private set; }

    public DateTimeOffset? PaidAtUtc { get; private set; }
}
