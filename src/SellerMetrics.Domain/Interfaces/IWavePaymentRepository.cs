using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for WavePayment with specialized operations.
/// </summary>
public interface IWavePaymentRepository : IRepository<WavePayment>
{
    /// <summary>
    /// Gets a payment by its Wave payment ID.
    /// </summary>
    /// <param name="wavePaymentId">The Wave payment ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment if found; otherwise, null.</returns>
    Task<WavePayment?> GetByWavePaymentIdAsync(
        string wavePaymentId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments for a specific invoice.
    /// </summary>
    /// <param name="invoiceId">The local invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of payments for the invoice.</returns>
    Task<IReadOnlyList<WavePayment>> GetByInvoiceIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of payments for the user.</returns>
    Task<IReadOnlyList<WavePayment>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of payments within the date range.</returns>
    Task<IReadOnlyList<WavePayment>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total payments received for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="currency">The currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total payments amount.</returns>
    Task<decimal> GetTotalPaymentsAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a Wave payment already exists for a user.
    /// </summary>
    /// <param name="wavePaymentId">The Wave payment ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the payment exists.</returns>
    Task<bool> ExistsAsync(
        string wavePaymentId,
        string userId,
        CancellationToken cancellationToken = default);
}
