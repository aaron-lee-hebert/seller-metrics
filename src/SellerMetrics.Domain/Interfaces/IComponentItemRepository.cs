using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for ComponentItem with specialized operations.
/// </summary>
public interface IComponentItemRepository : IRepository<ComponentItem>
{
    /// <summary>
    /// Gets component items by type.
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetByTypeAsync(
        int componentTypeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets component items by status.
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetByStatusAsync(
        ComponentStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets component items at a specific storage location.
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetByLocationAsync(
        int storageLocationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets components reserved for a specific service job.
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetByServiceJobAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets components with low stock (quantity at or below threshold).
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetLowStockAsync(
        int threshold = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total value of available components.
    /// </summary>
    Task<decimal> GetTotalComponentValueAsync(
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a component with its type and adjustment history.
    /// </summary>
    Task<ComponentItem?> GetWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets component items that have been soft-deleted beyond the retention period.
    /// </summary>
    Task<IReadOnlyList<ComponentItem>> GetExpiredDeletedAsync(
        int retentionDays = 30,
        CancellationToken cancellationToken = default);
}
