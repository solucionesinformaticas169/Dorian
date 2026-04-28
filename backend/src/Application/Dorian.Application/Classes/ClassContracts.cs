namespace Dorian.Application.Classes;

using Dorian.Modules.Classes.Domain.Entities;

public sealed record ClassSessionResponse(
    Guid Id,
    Guid BranchId,
    Guid? TrainerUserId,
    string Name,
    string? Description,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int Capacity,
    ClassSessionStatus Status,
    int ReservedSpots,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CreateClassSessionRequest(
    Guid BranchId,
    Guid? TrainerUserId,
    string Name,
    string? Description,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int Capacity,
    ClassSessionStatus Status);

public sealed record UpdateClassSessionRequest(
    Guid BranchId,
    Guid? TrainerUserId,
    string Name,
    string? Description,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int Capacity,
    ClassSessionStatus Status);
