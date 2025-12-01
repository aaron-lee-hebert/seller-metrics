using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for WaveUserCredential with specialized operations.
/// </summary>
public interface IWaveUserCredentialRepository : IRepository<WaveUserCredential>
{
    /// <summary>
    /// Gets credentials by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The credentials if found; otherwise, null.</returns>
    Task<WaveUserCredential?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all connected Wave users for sync operations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of connected user credentials.</returns>
    Task<IReadOnlyList<WaveUserCredential>> GetAllConnectedAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user already has Wave credentials.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if credentials exist.</returns>
    Task<bool> ExistsForUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
