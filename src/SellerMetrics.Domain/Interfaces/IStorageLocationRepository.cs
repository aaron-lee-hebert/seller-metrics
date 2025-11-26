using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for StorageLocation with hierarchy-specific operations.
/// </summary>
public interface IStorageLocationRepository : IRepository<StorageLocation>
{
    /// <summary>
    /// Gets all top-level storage locations (those without a parent).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of root storage locations.</returns>
    Task<IReadOnlyList<StorageLocation>> GetRootLocationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a storage location with all its children loaded recursively.
    /// </summary>
    /// <param name="id">The storage location ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The storage location with children, or null if not found.</returns>
    Task<StorageLocation?> GetWithChildrenAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full hierarchy of storage locations as a tree structure.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of root locations with their descendants.</returns>
    Task<IReadOnlyList<StorageLocation>> GetHierarchyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a storage location with its parent chain loaded (for displaying full path).
    /// </summary>
    /// <param name="id">The storage location ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The storage location with ancestors, or null if not found.</returns>
    Task<StorageLocation?> GetWithAncestorsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a storage location has any child locations.
    /// </summary>
    /// <param name="id">The storage location ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if children exist; otherwise, false.</returns>
    Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a storage location has any items (inventory or components) stored in it.
    /// </summary>
    /// <param name="id">The storage location ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if items exist; otherwise, false.</returns>
    Task<bool> HasItemsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage locations that have been soft-deleted beyond the retention period.
    /// </summary>
    /// <param name="retentionDays">Number of days after which deleted items can be purged (default 30).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of locations eligible for permanent deletion.</returns>
    Task<IReadOnlyList<StorageLocation>> GetExpiredDeletedAsync(int retentionDays = 30, CancellationToken cancellationToken = default);
}
