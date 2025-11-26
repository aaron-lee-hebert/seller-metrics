using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for mileage entry operations.
/// </summary>
public interface IMileageEntryRepository : IRepository<MileageEntry>
{
    /// <summary>
    /// Gets mileage entries within a date range.
    /// </summary>
    Task<IReadOnlyList<MileageEntry>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets mileage entries by business line.
    /// </summary>
    Task<IReadOnlyList<MileageEntry>> GetByBusinessLineAsync(
        BusinessLine businessLine,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets mileage entries by business line within a date range.
    /// </summary>
    Task<IReadOnlyList<MileageEntry>> GetByBusinessLineAndDateRangeAsync(
        BusinessLine businessLine,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets mileage entries for a specific service job.
    /// </summary>
    Task<IReadOnlyList<MileageEntry>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total miles for a date range.
    /// </summary>
    Task<decimal> GetTotalMilesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total miles by business line for a date range.
    /// </summary>
    Task<decimal> GetTotalMilesByBusinessLineAsync(
        BusinessLine businessLine,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
