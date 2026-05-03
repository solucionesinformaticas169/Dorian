namespace Dorian.Modules.Customers.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class BodyProgressPhoto : AuditableEntity<Guid>
{
    private BodyProgressPhoto() : base(Guid.Empty)
    {
    }

    public BodyProgressPhoto(
        Guid id,
        Guid customerId,
        string photoUrl,
        DateTimeOffset takenAt,
        BodyProgressPhotoType type,
        string? notes) : base(id)
    {
        CustomerId = customerId;
        Update(photoUrl, takenAt, type, notes);
    }

    public Guid CustomerId { get; private set; }
    public string PhotoUrl { get; private set; } = string.Empty;
    public DateTimeOffset TakenAt { get; private set; }
    public BodyProgressPhotoType Type { get; private set; }
    public string? Notes { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public void Update(string photoUrl, DateTimeOffset takenAt, BodyProgressPhotoType type, string? notes)
    {
        PhotoUrl = photoUrl.Trim();
        TakenAt = takenAt;
        Type = type;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
