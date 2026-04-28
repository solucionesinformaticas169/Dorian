namespace Dorian.Application.Access;

using Dorian.Modules.Access.Domain.Entities;

public sealed record AccessPassResponse(
    Guid Id,
    Guid CustomerId,
    string QrCodeValue,
    DateTimeOffset ExpiresAt,
    AccessPassStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CheckInResponse(
    Guid Id,
    Guid CustomerId,
    Guid BranchId,
    DateTimeOffset CheckedInAt,
    Guid? CheckedInByUserId,
    CheckInSource Source,
    CheckInStatus Status,
    string? RejectionReason,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record ScanCheckInRequest(string QrCodeValue);

public sealed record ManualCheckInRequest(Guid CustomerId);
