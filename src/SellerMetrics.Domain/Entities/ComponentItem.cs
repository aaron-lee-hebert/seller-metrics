using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a computer repair component item.
/// </summary>
public class ComponentItem : BaseEntity
{
    /// <summary>
    /// Component type ID (e.g., RAM, SSD, etc.).
    /// </summary>
    public int ComponentTypeId { get; set; }

    /// <summary>
    /// Component type reference.
    /// </summary>
    public ComponentType ComponentType { get; set; } = null!;

    /// <summary>
    /// Specific description (e.g., "8GB DDR4 2666MHz", "500GB Samsung 860 EVO").
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of this component in stock.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Cost per unit.
    /// </summary>
    public Money UnitCost { get; set; } = Money.Zero();

    /// <summary>
    /// Storage location ID where this component is stored.
    /// </summary>
    public int? StorageLocationId { get; set; }

    /// <summary>
    /// Storage location where this component is stored.
    /// </summary>
    public StorageLocation? StorageLocation { get; set; }

    /// <summary>
    /// Current status of the component.
    /// </summary>
    public ComponentStatus Status { get; set; } = ComponentStatus.Available;

    /// <summary>
    /// Date the component was acquired.
    /// </summary>
    public DateTime? AcquiredDate { get; set; }

    /// <summary>
    /// How the component was acquired.
    /// </summary>
    public ComponentSource Source { get; set; } = ComponentSource.Purchased;

    /// <summary>
    /// Additional notes about the component.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Low stock threshold for this component.
    /// When quantity falls at or below this value, the component is considered low stock.
    /// Null means no low stock alert for this component.
    /// </summary>
    public int? LowStockThreshold { get; set; }

    /// <summary>
    /// Service job ID if the component is reserved or used for a job.
    /// </summary>
    public int? ServiceJobId { get; set; }

    /// <summary>
    /// Service job this component is reserved for or used in.
    /// </summary>
    public ServiceJob? ServiceJob { get; set; }

    /// <summary>
    /// History of quantity adjustments for audit trail.
    /// </summary>
    public ICollection<ComponentQuantityAdjustment> Adjustments { get; set; } = new List<ComponentQuantityAdjustment>();

    /// <summary>
    /// Gets the total value of this component (Quantity × UnitCost).
    /// </summary>
    public Money TotalValue => UnitCost.Multiply(Quantity);

    /// <summary>
    /// Gets whether this component is at or below its low stock threshold.
    /// Returns false if no threshold is set.
    /// </summary>
    public bool IsLowStock => LowStockThreshold.HasValue &&
                              Status == ComponentStatus.Available &&
                              Quantity <= LowStockThreshold.Value;

    /// <summary>
    /// Adjusts the quantity and creates an audit record.
    /// </summary>
    public ComponentQuantityAdjustment AdjustQuantity(int adjustment, string reason)
    {
        var previousQuantity = Quantity;
        Quantity += adjustment;

        if (Quantity < 0)
        {
            Quantity = previousQuantity;
            throw new InvalidOperationException($"Cannot adjust quantity by {adjustment}: would result in negative quantity.");
        }

        UpdatedAt = DateTime.UtcNow;

        return new ComponentQuantityAdjustment
        {
            ComponentItemId = Id,
            PreviousQuantity = previousQuantity,
            NewQuantity = Quantity,
            Adjustment = adjustment,
            Reason = reason,
            AdjustedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Reserves the component for a service job.
    /// </summary>
    public void ReserveForJob(int serviceJobId)
    {
        if (Status != ComponentStatus.Available)
        {
            throw new InvalidOperationException($"Cannot reserve component: status is {Status}.");
        }

        Status = ComponentStatus.Reserved;
        ServiceJobId = serviceJobId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the component as used in a service job.
    /// </summary>
    public void MarkAsUsed()
    {
        if (Status != ComponentStatus.Reserved)
        {
            throw new InvalidOperationException($"Cannot mark as used: component must be reserved first (current status: {Status}).");
        }

        Status = ComponentStatus.Used;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Releases the reservation on a component.
    /// </summary>
    public void ReleaseReservation()
    {
        if (Status != ComponentStatus.Reserved)
        {
            throw new InvalidOperationException($"Cannot release reservation: component is not reserved (current status: {Status}).");
        }

        Status = ComponentStatus.Available;
        ServiceJobId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Moves the component to a new storage location.
    /// </summary>
    public void MoveTo(int? storageLocationId)
    {
        StorageLocationId = storageLocationId;
        UpdatedAt = DateTime.UtcNow;
    }
}
