namespace Dorian.Application.Dashboard;

public sealed record DashboardSummaryResponse(
    int ActiveCustomersCount,
    int TodayClassesCount,
    int TodayCheckInsCount,
    decimal EstimatedRevenue,
    string MostActiveBranchName,
    IReadOnlyCollection<BranchActivityPoint> BranchActivity,
    IReadOnlyCollection<ClassOccupancyPoint> ClassOccupancy,
    string EstimatedRevenueFormula);

public sealed record BranchActivityPoint(
    Guid BranchId,
    string BranchName,
    int ActivityCount,
    int ActiveCustomersCount,
    int TodayClassesCount,
    int TodayCheckInsCount);

public sealed record ClassOccupancyPoint(
    Guid ClassSessionId,
    string ClassName,
    string BranchName,
    DateTimeOffset StartTime,
    int ReservedSpots,
    int Capacity,
    decimal OccupancyRate);
