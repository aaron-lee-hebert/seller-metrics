using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a storage location with support for arbitrary depth hierarchy.
/// Examples: "Garage > Shelf A > Bin 3", "Office > Closet > Top Shelf"
/// </summary>
public class StorageLocation : BaseEntity
{
    /// <summary>
    /// Name of this storage location (e.g., "Garage", "Shelf A", "Bin 3").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description or notes about this location.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Parent location ID for hierarchical structure. Null for top-level locations.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Parent storage location. Null for top-level locations.
    /// </summary>
    public StorageLocation? Parent { get; set; }

    /// <summary>
    /// Child storage locations.
    /// </summary>
    public ICollection<StorageLocation> Children { get; set; } = new List<StorageLocation>();

    /// <summary>
    /// Sort order for display purposes within the same parent level.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets the full hierarchical path (e.g., "Garage > Shelf A > Bin 3").
    /// This is a computed property that traverses up the hierarchy.
    /// </summary>
    public string FullPath
    {
        get
        {
            var parts = new List<string> { Name };
            var current = Parent;
            while (current != null)
            {
                parts.Insert(0, current.Name);
                current = current.Parent;
            }
            return string.Join(" > ", parts);
        }
    }

    /// <summary>
    /// Gets the depth level in the hierarchy (0 for root, 1 for first level children, etc.).
    /// </summary>
    public int Depth
    {
        get
        {
            var depth = 0;
            var current = Parent;
            while (current != null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }
    }
}
