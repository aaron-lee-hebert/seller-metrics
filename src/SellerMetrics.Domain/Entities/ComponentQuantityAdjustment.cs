namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Audit record for component quantity adjustments.
/// </summary>
public class ComponentQuantityAdjustment
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Component item that was adjusted.
    /// </summary>
    public int ComponentItemId { get; set; }

    /// <summary>
    /// Reference to the component item.
    /// </summary>
    public ComponentItem ComponentItem { get; set; } = null!;

    /// <summary>
    /// Quantity before the adjustment.
    /// </summary>
    public int PreviousQuantity { get; set; }

    /// <summary>
    /// Quantity after the adjustment.
    /// </summary>
    public int NewQuantity { get; set; }

    /// <summary>
    /// The adjustment amount (positive for add, negative for remove).
    /// </summary>
    public int Adjustment { get; set; }

    /// <summary>
    /// Reason for the adjustment.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Date and time of the adjustment.
    /// </summary>
    public DateTime AdjustedAt { get; set; } = DateTime.UtcNow;
}
