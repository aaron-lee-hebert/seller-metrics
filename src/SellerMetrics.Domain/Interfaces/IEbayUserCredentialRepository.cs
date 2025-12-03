using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for EbayUserCredential with specialized operations.
/// </summary>
public interface IEbayUserCredentialRepository : IRepository<EbayUserCredential>
{
    /// <summary>
    /// Gets the eBay credentials for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's eBay credentials if found; otherwise, null.</returns>
    Task<EbayUserCredential?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all connected users (for background sync).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of connected user credentials.</returns>
    Task<IReadOnlyList<EbayUserCredential>> GetAllConnectedAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users with expired access tokens that need refresh.
    /// Only returns users whose refresh tokens are still valid.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of credentials needing token refresh.</returns>
    Task<IReadOnlyList<EbayUserCredential>> GetNeedingTokenRefreshAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users with expired refresh tokens (need to re-authenticate).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of credentials with expired refresh tokens.</returns>
    Task<IReadOnlyList<EbayUserCredential>> GetWithExpiredRefreshTokensAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has connected their eBay account.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has a connected eBay account.</returns>
    Task<bool> IsUserConnectedAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
