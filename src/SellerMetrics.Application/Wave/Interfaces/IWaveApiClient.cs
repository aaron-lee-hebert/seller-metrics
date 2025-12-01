using SellerMetrics.Application.Wave.DTOs;

namespace SellerMetrics.Application.Wave.Interfaces;

/// <summary>
/// Interface for Wave API client operations.
/// </summary>
public interface IWaveApiClient
{
    /// <summary>
    /// Gets available businesses for the user's Wave account.
    /// </summary>
    /// <param name="accessToken">The Wave Full Access Token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of businesses.</returns>
    Task<IReadOnlyList<WaveBusinessDto>> GetBusinessesAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets invoices for a business within a date range.
    /// </summary>
    /// <param name="accessToken">The Wave Full Access Token.</param>
    /// <param name="businessId">The Wave business ID.</param>
    /// <param name="startDate">The start date for filtering invoices.</param>
    /// <param name="endDate">The end date for filtering invoices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of invoices.</returns>
    Task<IReadOnlyList<WaveInvoiceDto>> GetInvoicesAsync(
        string accessToken,
        string businessId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific invoice by ID.
    /// </summary>
    /// <param name="accessToken">The Wave Full Access Token.</param>
    /// <param name="businessId">The Wave business ID.</param>
    /// <param name="invoiceId">The Wave invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice details.</returns>
    Task<WaveInvoiceDto?> GetInvoiceAsync(
        string accessToken,
        string businessId,
        string invoiceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the access token by attempting to fetch businesses.
    /// </summary>
    /// <param name="accessToken">The Wave Full Access Token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}
