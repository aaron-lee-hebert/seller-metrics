using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Inventory.DTOs;

/// <summary>
/// Data transfer object for InventoryItem.
/// </summary>
public class InventoryItemDto
{
    public int Id { get; set; }
    public string InternalSku { get; set; } = string.Empty;
    public string? EbaySku { get; set; }
    public string EffectiveSku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CogsAmount { get; set; }
    public string CogsCurrency { get; set; } = "USD";
    public string CogsFormatted { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal TotalValueAmount { get; set; }
    public string TotalValueFormatted { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public int? StorageLocationId { get; set; }
    public string? StorageLocationPath { get; set; }
    public InventoryStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public EbayCondition? Condition { get; set; }
    public string? ConditionDisplay { get; set; }
    public string? Notes { get; set; }
    public string? PhotoPath { get; set; }
    public string? EbayListingId { get; set; }
    public DateTime? ListedDate { get; set; }
    public DateTime? SoldDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Summary DTO for inventory listings.
/// </summary>
public class InventoryItemSummaryDto
{
    public int Id { get; set; }
    public string EffectiveSku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CogsFormatted { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string TotalValueFormatted { get; set; } = string.Empty;
    public InventoryStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string? StorageLocationPath { get; set; }
}

/// <summary>
/// DTO for inventory value summary.
/// </summary>
public class InventoryValueDto
{
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalValueFormatted { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
    public int UnlistedCount { get; set; }
    public int ListedCount { get; set; }
}
