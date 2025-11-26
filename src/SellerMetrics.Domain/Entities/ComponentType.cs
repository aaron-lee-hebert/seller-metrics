using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Catalog of component/part types.
/// Can be predefined (seeded) or user-created.
/// </summary>
public class ComponentType : BaseEntity
{
    /// <summary>
    /// Name of the component type (e.g., "RAM", "SSD", "Power Supply").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the component type.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Default expense category for purchased components of this type.
    /// Maps to IRS Schedule C categories.
    /// </summary>
    public string? DefaultExpenseCategory { get; set; }

    /// <summary>
    /// Whether this type was created by the system (seeded) or user.
    /// System types cannot be deleted.
    /// </summary>
    public bool IsSystemType { get; set; }

    /// <summary>
    /// Sort order for display purposes.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Component items of this type.
    /// </summary>
    public ICollection<ComponentItem> Items { get; set; } = new List<ComponentItem>();
}
