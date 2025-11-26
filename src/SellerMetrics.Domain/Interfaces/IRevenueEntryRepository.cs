using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for revenue entry operations.
/// </summary>
public interface IRevenueEntryRepository : IRepository<RevenueEntry>
{
    /// <summary>
    /// Gets revenue entries by source.
    /// </summary>
    Task<IReadOnlyList<RevenueEntry>> GetBySourceAsync(
        RevenueSource source,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets revenue entries within a date range.
    /// </summary>
    Task<IReadOnlyList<RevenueEntry>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets revenue entries by source within a date range.
    /// </summary>
    Task<IReadOnlyList<RevenueEntry>> GetBySourceAndDateRangeAsync(
        RevenueSource source,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a revenue entry by eBay order ID.
    /// </summary>
    Task<RevenueEntry?> GetByEbayOrderIdAsync(
        string ebayOrderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a revenue entry by Wave invoice number.
    /// </summary>
    Task<RevenueEntry?> GetByWaveInvoiceNumberAsync(
        string waveInvoiceNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets revenue entries for a service job.
    /// </summary>
    Task<IReadOnlyList<RevenueEntry>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total revenue amount for a date range.
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total revenue by source for a date range.
    /// </summary>
    Task<decimal> GetTotalRevenueBySourceAsync(
        RevenueSource source,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets monthly revenue totals for a date range.
    /// </summary>
    Task<IReadOnlyList<(int Year, int Month, decimal Total)>> GetMonthlyRevenueTotalsAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);
}
