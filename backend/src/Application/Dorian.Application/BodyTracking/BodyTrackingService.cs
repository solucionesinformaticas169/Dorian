namespace Dorian.Application.BodyTracking;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;

public sealed class BodyTrackingService : IBodyTrackingService
{
    private readonly IDorianDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public BodyTrackingService(IDorianDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<BodyMeasurementResponse>> GetMyMeasurementsAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await GetMeasurementsForCustomerAsync(customer.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BodyMeasurementResponse>> GetMeasurementsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForViewAsync(customerId, cancellationToken);
        return await GetMeasurementsForCustomerAsync(customer.Id, cancellationToken);
    }

    public async Task<BodyMeasurementResponse?> GetMyLatestMeasurementAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var measurement = await _dbContext.BodyMeasurements
            .AsNoTracking()
            .Where(x => x.CustomerId == customer.Id)
            .OrderByDescending(x => x.MeasuredAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return measurement is null ? null : Map(measurement);
    }

    public async Task<BodyMeasurementResponse> CreateMeasurementForCurrentCustomerAsync(SaveBodyMeasurementRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var measurement = new BodyMeasurement(
            Guid.NewGuid(),
            customer.Id,
            request.MeasuredAt,
            request.WeightKg,
            request.HeightCm,
            request.BodyFatPercentage,
            request.MuscleMassKg,
            request.BoneMassKg,
            request.ResidualMassKg,
            request.WaistCm,
            request.ChestCm,
            request.HipCm,
            request.ShouldersCm,
            request.LeftArmCm,
            request.RightArmCm,
            request.LeftLegCm,
            request.RightLegCm,
            request.LeftCalfCm,
            request.RightCalfCm,
            request.NeckCm,
            request.Notes);

        _dbContext.BodyMeasurements.Add(measurement);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(measurement);
    }

    public async Task<BodyMeasurementResponse> UpdateMeasurementForCurrentCustomerAsync(Guid measurementId, SaveBodyMeasurementRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var measurement = await _dbContext.BodyMeasurements.SingleOrDefaultAsync(x => x.Id == measurementId && x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NotFoundException("Body measurement not found.");

        measurement.Update(
            request.MeasuredAt,
            request.WeightKg,
            request.HeightCm,
            request.BodyFatPercentage,
            request.MuscleMassKg,
            request.BoneMassKg,
            request.ResidualMassKg,
            request.WaistCm,
            request.ChestCm,
            request.HipCm,
            request.ShouldersCm,
            request.LeftArmCm,
            request.RightArmCm,
            request.LeftLegCm,
            request.RightLegCm,
            request.LeftCalfCm,
            request.RightCalfCm,
            request.NeckCm,
            request.Notes);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(measurement);
    }

    public async Task DeleteMeasurementForCurrentCustomerAsync(Guid measurementId, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var measurement = await _dbContext.BodyMeasurements.SingleOrDefaultAsync(x => x.Id == measurementId && x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NotFoundException("Body measurement not found.");

        _dbContext.BodyMeasurements.Remove(measurement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<BodyProgressPhotoResponse>> GetMyProgressPhotosAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await _dbContext.BodyProgressPhotos
            .AsNoTracking()
            .Where(x => x.CustomerId == customer.Id)
            .OrderByDescending(x => x.TakenAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<BodyProgressPhotoResponse> CreateProgressPhotoForCurrentCustomerAsync(SaveBodyProgressPhotoRequest request, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var photo = new BodyProgressPhoto(Guid.NewGuid(), customer.Id, request.PhotoUrl, request.TakenAt, request.Type, request.Notes);
        _dbContext.BodyProgressPhotos.Add(photo);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(photo);
    }

    public async Task DeleteProgressPhotoForCurrentCustomerAsync(Guid photoId, CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var photo = await _dbContext.BodyProgressPhotos.SingleOrDefaultAsync(x => x.Id == photoId && x.CustomerId == customer.Id, cancellationToken)
            ?? throw new NotFoundException("Body progress photo not found.");
        _dbContext.BodyProgressPhotos.Remove(photo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<BodySummaryResponse> GetMyBodySummaryAsync(CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return await BuildSummaryAsync(customer, cancellationToken);
    }

    public async Task<BodySummaryResponse> GetBodySummaryByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerForViewAsync(customerId, cancellationToken);
        return await BuildSummaryAsync(customer, cancellationToken);
    }

    private async Task<BodySummaryResponse> BuildSummaryAsync(Customer customer, CancellationToken cancellationToken)
    {
        var measurements = await _dbContext.BodyMeasurements
            .AsNoTracking()
            .Where(x => x.CustomerId == customer.Id)
            .OrderByDescending(x => x.MeasuredAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var photos = await _dbContext.BodyProgressPhotos
            .AsNoTracking()
            .Where(x => x.CustomerId == customer.Id)
            .OrderByDescending(x => x.TakenAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);

        var fitnessProfile = await _dbContext.CustomerFitnessProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);

        var latest = measurements.FirstOrDefault();
        var currentWeight = latest?.WeightKg ?? fitnessProfile?.WeightKg;
        var heightCm = latest?.HeightCm ?? fitnessProfile?.HeightCm;
        var bmi = CalculateBmi(currentWeight, heightCm);
        var latestDate = latest?.MeasuredAt;

        return new BodySummaryResponse(
            currentWeight,
            fitnessProfile?.TargetWeightKg,
            heightCm,
            bmi,
            GetBmiLabel(bmi),
            latestDate,
            measurements
                .OrderBy(x => x.MeasuredAt)
                .Select(x => new BodyWeightHistoryPoint(x.MeasuredAt, x.WeightKg, CalculateBmi(x.WeightKg, x.HeightCm) ?? 0))
                .ToList(),
            measurements
                .OrderBy(x => x.MeasuredAt)
                .Select(x => new BodyMeasurementsHistoryPoint(
                    x.MeasuredAt,
                    x.WaistCm,
                    x.ChestCm,
                    x.HipCm,
                    x.ShouldersCm,
                    x.LeftArmCm,
                    x.RightArmCm,
                    x.LeftLegCm,
                    x.RightLegCm,
                    x.LeftCalfCm,
                    x.RightCalfCm,
                    x.NeckCm,
                    x.BodyFatPercentage,
                    x.MuscleMassKg))
                .ToList(),
            photos,
            latestDate.HasValue ? Math.Max(0, (int)(DateTimeOffset.UtcNow.Date - latestDate.Value.UtcDateTime.Date).TotalDays) : null);
    }

    private async Task<IReadOnlyCollection<BodyMeasurementResponse>> GetMeasurementsForCustomerAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.BodyMeasurements
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.MeasuredAt)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);
    }

    private async Task<Customer> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        var user = EnsureAuthenticated();
        if (!user.IsInRole(RoleNames.Customer) || !user.UserId.HasValue)
        {
            throw new ForbiddenException("Only customers can manage their own body tracking.");
        }

        return await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == user.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Customer profile not found.");
    }

    private async Task<Customer> GetCustomerForViewAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");
        EnsureCanViewCustomer(customer);
        return customer;
    }

    private void EnsureCanViewCustomer(Customer customer)
    {
        var user = EnsureAuthenticated();
        if (user.IsInRole(RoleNames.SuperAdmin)) return;
        if (user.IsInRole(RoleNames.Customer) && user.UserId == customer.UserId) return;
        if ((user.IsInRole(RoleNames.BranchAdmin) || user.IsInRole(RoleNames.Reception) || user.IsInRole(RoleNames.Trainer)) && user.BranchId == customer.BranchId) return;
        throw new ForbiddenException("You cannot view this customer's body tracking.");
    }

    private CurrentUser EnsureAuthenticated()
    {
        var user = _currentUserService.User;
        if (!user.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        return user;
    }

    private static BodyMeasurementResponse Map(BodyMeasurement measurement) => new(
        measurement.Id,
        measurement.CustomerId,
        measurement.MeasuredAt,
        measurement.WeightKg,
        measurement.HeightCm,
        measurement.BodyFatPercentage,
        measurement.MuscleMassKg,
        measurement.BoneMassKg,
        measurement.ResidualMassKg,
        CalculateBmi(measurement.WeightKg, measurement.HeightCm) ?? 0,
        measurement.WaistCm,
        measurement.ChestCm,
        measurement.HipCm,
        measurement.ShouldersCm,
        measurement.LeftArmCm,
        measurement.RightArmCm,
        measurement.LeftLegCm,
        measurement.RightLegCm,
        measurement.LeftCalfCm,
        measurement.RightCalfCm,
        measurement.NeckCm,
        measurement.Notes,
        measurement.CreatedAtUtc,
        measurement.UpdatedAtUtc);

    private static BodyProgressPhotoResponse Map(BodyProgressPhoto photo) => new(
        photo.Id,
        photo.CustomerId,
        photo.PhotoUrl,
        photo.TakenAt,
        photo.Type,
        photo.Notes,
        photo.CreatedAtUtc);

    private static decimal? CalculateBmi(decimal? weightKg, decimal? heightCm)
    {
        if (!weightKg.HasValue || !heightCm.HasValue || weightKg <= 0 || heightCm <= 0) return null;
        var heightMeters = heightCm.Value / 100m;
        if (heightMeters <= 0) return null;
        return Math.Round(weightKg.Value / (heightMeters * heightMeters), 2, MidpointRounding.AwayFromZero);
    }

    private static string GetBmiLabel(decimal? bmi)
    {
        if (!bmi.HasValue) return "Sin datos";
        if (bmi.Value < 18.5m) return "Bajo peso";
        if (bmi.Value < 25m) return "Saludable";
        if (bmi.Value < 30m) return "Sobrepeso";
        return "Obesidad";
    }
}
