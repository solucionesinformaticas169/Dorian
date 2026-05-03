namespace Dorian.Application.BodyTracking;

using Dorian.Modules.Customers.Domain.Entities;

public sealed record SaveBodyMeasurementRequest(
    DateTimeOffset MeasuredAt,
    decimal WeightKg,
    decimal HeightCm,
    decimal? BodyFatPercentage,
    decimal? MuscleMassKg,
    decimal? BoneMassKg,
    decimal? ResidualMassKg,
    decimal? WaistCm,
    decimal? ChestCm,
    decimal? HipCm,
    decimal? ShouldersCm,
    decimal? LeftArmCm,
    decimal? RightArmCm,
    decimal? LeftLegCm,
    decimal? RightLegCm,
    decimal? LeftCalfCm,
    decimal? RightCalfCm,
    decimal? NeckCm,
    string? Notes);

public sealed record BodyMeasurementResponse(
    Guid Id,
    Guid CustomerId,
    DateTimeOffset MeasuredAt,
    decimal WeightKg,
    decimal HeightCm,
    decimal? BodyFatPercentage,
    decimal? MuscleMassKg,
    decimal? BoneMassKg,
    decimal? ResidualMassKg,
    decimal Bmi,
    decimal? WaistCm,
    decimal? ChestCm,
    decimal? HipCm,
    decimal? ShouldersCm,
    decimal? LeftArmCm,
    decimal? RightArmCm,
    decimal? LeftLegCm,
    decimal? RightLegCm,
    decimal? LeftCalfCm,
    decimal? RightCalfCm,
    decimal? NeckCm,
    string? Notes,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record SaveBodyProgressPhotoRequest(
    string PhotoUrl,
    DateTimeOffset TakenAt,
    BodyProgressPhotoType Type,
    string? Notes);

public sealed record BodyProgressPhotoResponse(
    Guid Id,
    Guid CustomerId,
    string PhotoUrl,
    DateTimeOffset TakenAt,
    BodyProgressPhotoType Type,
    string? Notes,
    DateTimeOffset CreatedAtUtc);

public sealed record BodyWeightHistoryPoint(
    DateTimeOffset MeasuredAt,
    decimal WeightKg,
    decimal Bmi);

public sealed record BodyMeasurementsHistoryPoint(
    DateTimeOffset MeasuredAt,
    decimal? WaistCm,
    decimal? ChestCm,
    decimal? HipCm,
    decimal? ShouldersCm,
    decimal? LeftArmCm,
    decimal? RightArmCm,
    decimal? LeftLegCm,
    decimal? RightLegCm,
    decimal? LeftCalfCm,
    decimal? RightCalfCm,
    decimal? NeckCm,
    decimal? BodyFatPercentage,
    decimal? MuscleMassKg);

public sealed record BodySummaryResponse(
    decimal? CurrentWeightKg,
    decimal? TargetWeightKg,
    decimal? HeightCm,
    decimal? Bmi,
    string BmiLabel,
    DateTimeOffset? LatestMeasurementDate,
    IReadOnlyCollection<BodyWeightHistoryPoint> WeightHistory,
    IReadOnlyCollection<BodyMeasurementsHistoryPoint> MeasurementsHistory,
    IReadOnlyCollection<BodyProgressPhotoResponse> ProgressPhotos,
    int? DaysSinceLastMeasurement);
