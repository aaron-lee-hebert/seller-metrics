using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a computer repair component item.
/// Placeholder - full implementation in Component Inventory subsection.
/// </summary>
public class ComponentItem : BaseEntity
{
    /// <summary>
    /// Storage location ID where this component is stored.
    /// </summary>
    public int? StorageLocationId { get; set; }

    /// <summary>
    /// Storage location where this component is stored.
    /// </summary>
    public StorageLocation? StorageLocation { get; set; }
}
