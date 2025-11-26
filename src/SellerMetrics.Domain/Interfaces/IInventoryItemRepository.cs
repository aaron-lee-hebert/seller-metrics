using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for InventoryItem with specialized operations.
/// </summary>
public interface IInventoryItemRepository : IRepository<InventoryItem>
{
    /// <summary>
    /// Gets inventory items filtered by status.
    /// </summary>
    Task<IReadOnlyList<InventoryItem>> GetByStatusAsync(
        InventoryStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory items at a specific storage location.
    /// </summary>
    Task<IReadOnlyList<InventoryItem>> GetByLocationAsync(
        int storageLocationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an inventory item by its internal SKU.
    /// </summary>
    Task<InventoryItem?> GetByInternalSkuAsync(
        string internalSku,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an inventory item by its eBay SKU.
    /// </summary>
    Task<InventoryItem?> GetByEbaySkuAsync(
        string ebaySku,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an inventory item by its eBay listing ID.
    /// </summary>
    Task<InventoryItem?> GetByEbayListingIdAsync(
        string ebayListingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches inventory items by title, SKU, or notes.
    /// </summary>
    Task<IReadOnlyList<InventoryItem>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total COGS value of unsold inventory (status != Sold).
    /// </summary>
    Task<decimal> GetTotalInventoryValueAsync(
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next sequence number for SKU generation.
    /// </summary>
    Task<int> GetNextSkuSequenceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an internal SKU already exists.
    /// </summary>
    Task<bool> InternalSkuExistsAsync(
        string internalSku,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an eBay SKU already exists (excluding a specific item).
    /// </summary>
    Task<bool> EbaySkuExistsAsync(
        string ebaySku,
        int? excludeItemId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory items that have been soft-deleted beyond the retention period.
    /// </summary>
    Task<IReadOnlyList<InventoryItem>> GetExpiredDeletedAsync(
        int retentionDays = 30,
        CancellationToken cancellationToken = default);
}
