using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for WaveInvoice with specialized operations.
/// </summary>
public interface IWaveInvoiceRepository : IRepository<WaveInvoice>
{
    /// <summary>
    /// Gets an invoice by its Wave invoice ID.
    /// </summary>
    /// <param name="waveInvoiceId">The Wave invoice ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice if found; otherwise, null.</returns>
    Task<WaveInvoice?> GetByWaveInvoiceIdAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invoices for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of invoices for the user.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invoices for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of invoices within the date range.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invoices by status for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="status">The invoice status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of invoices with the specified status.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetByStatusAsync(
        string userId,
        WaveInvoiceStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unpaid invoices for a user (excludes Paid and Voided).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of unpaid invoices.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetUnpaidInvoicesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue invoices for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of overdue invoices.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetOverdueInvoicesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total revenue from paid invoices for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="currency">The currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total revenue amount.</returns>
    Task<decimal> GetTotalRevenueAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invoice count for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of invoices.</returns>
    Task<int> GetInvoiceCountAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent invoices for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="count">The number of invoices to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of the most recent invoices.</returns>
    Task<IReadOnlyList<WaveInvoice>> GetRecentInvoicesAsync(
        string userId,
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a Wave invoice already exists for a user.
    /// </summary>
    /// <param name="waveInvoiceId">The Wave invoice ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the invoice exists.</returns>
    Task<bool> ExistsAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an invoice was previously deleted (soft-deleted).
    /// Used during sync to avoid re-creating deleted invoices.
    /// </summary>
    /// <param name="waveInvoiceId">The Wave invoice ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the invoice was previously deleted.</returns>
    Task<bool> WasDeletedAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default);
}
