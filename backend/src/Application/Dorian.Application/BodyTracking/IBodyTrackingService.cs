namespace Dorian.Application.BodyTracking;

public interface IBodyTrackingService
{
    Task<IReadOnlyCollection<BodyMeasurementResponse>> GetMyMeasurementsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<BodyMeasurementResponse>> GetMeasurementsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<BodyMeasurementResponse?> GetMyLatestMeasurementAsync(CancellationToken cancellationToken);
    Task<BodyMeasurementResponse> CreateMeasurementForCurrentCustomerAsync(SaveBodyMeasurementRequest request, CancellationToken cancellationToken);
    Task<BodyMeasurementResponse> UpdateMeasurementForCurrentCustomerAsync(Guid measurementId, SaveBodyMeasurementRequest request, CancellationToken cancellationToken);
    Task DeleteMeasurementForCurrentCustomerAsync(Guid measurementId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<BodyProgressPhotoResponse>> GetMyProgressPhotosAsync(CancellationToken cancellationToken);
    Task<BodyProgressPhotoResponse> CreateProgressPhotoForCurrentCustomerAsync(SaveBodyProgressPhotoRequest request, CancellationToken cancellationToken);
    Task DeleteProgressPhotoForCurrentCustomerAsync(Guid photoId, CancellationToken cancellationToken);
    Task<BodySummaryResponse> GetMyBodySummaryAsync(CancellationToken cancellationToken);
    Task<BodySummaryResponse> GetBodySummaryByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
