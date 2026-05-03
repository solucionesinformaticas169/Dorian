namespace Dorian.Modules.Customers.Domain.Entities;

using Dorian.SharedKernel.Primitives;

public sealed class BodyMeasurement : AuditableEntity<Guid>
{
    private BodyMeasurement() : base(Guid.Empty)
    {
    }

    public BodyMeasurement(
        Guid id,
        Guid customerId,
        DateTimeOffset measuredAt,
        decimal weightKg,
        decimal heightCm,
        decimal? bodyFatPercentage,
        decimal? muscleMassKg,
        decimal? boneMassKg,
        decimal? residualMassKg,
        decimal? waistCm,
        decimal? chestCm,
        decimal? hipCm,
        decimal? shouldersCm,
        decimal? leftArmCm,
        decimal? rightArmCm,
        decimal? leftLegCm,
        decimal? rightLegCm,
        decimal? leftCalfCm,
        decimal? rightCalfCm,
        decimal? neckCm,
        string? notes) : base(id)
    {
        CustomerId = customerId;
        Update(
            measuredAt,
            weightKg,
            heightCm,
            bodyFatPercentage,
            muscleMassKg,
            boneMassKg,
            residualMassKg,
            waistCm,
            chestCm,
            hipCm,
            shouldersCm,
            leftArmCm,
            rightArmCm,
            leftLegCm,
            rightLegCm,
            leftCalfCm,
            rightCalfCm,
            neckCm,
            notes);
    }

    public Guid CustomerId { get; private set; }
    public DateTimeOffset MeasuredAt { get; private set; }
    public decimal WeightKg { get; private set; }
    public decimal HeightCm { get; private set; }
    public decimal? BodyFatPercentage { get; private set; }
    public decimal? MuscleMassKg { get; private set; }
    public decimal? BoneMassKg { get; private set; }
    public decimal? ResidualMassKg { get; private set; }
    public decimal? WaistCm { get; private set; }
    public decimal? ChestCm { get; private set; }
    public decimal? HipCm { get; private set; }
    public decimal? ShouldersCm { get; private set; }
    public decimal? LeftArmCm { get; private set; }
    public decimal? RightArmCm { get; private set; }
    public decimal? LeftLegCm { get; private set; }
    public decimal? RightLegCm { get; private set; }
    public decimal? LeftCalfCm { get; private set; }
    public decimal? RightCalfCm { get; private set; }
    public decimal? NeckCm { get; private set; }
    public string? Notes { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public void Update(
        DateTimeOffset measuredAt,
        decimal weightKg,
        decimal heightCm,
        decimal? bodyFatPercentage,
        decimal? muscleMassKg,
        decimal? boneMassKg,
        decimal? residualMassKg,
        decimal? waistCm,
        decimal? chestCm,
        decimal? hipCm,
        decimal? shouldersCm,
        decimal? leftArmCm,
        decimal? rightArmCm,
        decimal? leftLegCm,
        decimal? rightLegCm,
        decimal? leftCalfCm,
        decimal? rightCalfCm,
        decimal? neckCm,
        string? notes)
    {
        MeasuredAt = measuredAt;
        WeightKg = weightKg;
        HeightCm = heightCm;
        BodyFatPercentage = bodyFatPercentage;
        MuscleMassKg = muscleMassKg;
        BoneMassKg = boneMassKg;
        ResidualMassKg = residualMassKg;
        WaistCm = waistCm;
        ChestCm = chestCm;
        HipCm = hipCm;
        ShouldersCm = shouldersCm;
        LeftArmCm = leftArmCm;
        RightArmCm = rightArmCm;
        LeftLegCm = leftLegCm;
        RightLegCm = rightLegCm;
        LeftCalfCm = leftCalfCm;
        RightCalfCm = rightCalfCm;
        NeckCm = neckCm;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
