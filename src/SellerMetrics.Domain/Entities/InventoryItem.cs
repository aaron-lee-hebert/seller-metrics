using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents an eBay inventory item.
/// Placeholder - full implementation in eBay Inventory subsection.
/// </summary>
public class InventoryItem : BaseEntity
{
    /// <summary>
    /// Storage location ID where this item is stored.
    /// </summary>
    public int? StorageLocationId { get; set; }

    /// <summary>
    /// Storage location where this item is stored.
    /// </summary>
    public StorageLocation? StorageLocation { get; set; }
}
