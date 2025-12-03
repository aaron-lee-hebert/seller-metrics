using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Components.DTOs;

/// <summary>
/// Data transfer object for ComponentItem.
/// </summary>
public class ComponentItemDto
{
    public int Id { get; set; }
    public int ComponentTypeId { get; set; }
    public string ComponentTypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCostAmount { get; set; }
    public string UnitCostCurrency { get; set; } = "USD";
    public string UnitCostFormatted { get; set; } = string.Empty;
    public decimal TotalValueAmount { get; set; }
    public string TotalValueFormatted { get; set; } = string.Empty;
    public int? StorageLocationId { get; set; }
    public string? StorageLocationPath { get; set; }
    public ComponentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public DateTime? AcquiredDate { get; set; }
    public ComponentSource Source { get; set; }
    public string SourceDisplay { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool IsLowStock { get; set; }
    public int? ServiceJobId { get; set; }
    public string? ServiceJobNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for component value summary.
/// </summary>
public class ComponentValueDto
{
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalValueFormatted { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
    public int LowStockCount { get; set; }
}

/// <summary>
/// DTO for quantity adjustment history.
/// </summary>
public class ComponentQuantityAdjustmentDto
{
    public int Id { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int Adjustment { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime AdjustedAt { get; set; }
}
