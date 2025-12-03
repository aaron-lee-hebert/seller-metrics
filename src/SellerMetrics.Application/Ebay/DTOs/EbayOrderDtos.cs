using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Ebay.DTOs;

/// <summary>
/// DTO for displaying eBay order information.
/// </summary>
public class EbayOrderDisplayDto
{
    public int Id { get; set; }
    public string EbayOrderId { get; set; } = string.Empty;
    public string? LegacyOrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string BuyerUsername { get; set; } = string.Empty;
    public string ItemTitle { get; set; } = string.Empty;
    public string? EbayItemId { get; set; }
    public string? Sku { get; set; }
    public int Quantity { get; set; }

    // Money fields
    public decimal GrossSaleAmount { get; set; }
    public string GrossSaleFormatted { get; set; } = string.Empty;
    public decimal ShippingPaidAmount { get; set; }
    public string ShippingPaidFormatted { get; set; } = string.Empty;
    public decimal ShippingActualAmount { get; set; }
    public string ShippingActualFormatted { get; set; } = string.Empty;
    public decimal TotalFeesAmount { get; set; }
    public string TotalFeesFormatted { get; set; } = string.Empty;
    public decimal NetPayoutAmount { get; set; }
    public string NetPayoutFormatted { get; set; } = string.Empty;

    // Profit fields (null if not linked to inventory)
    public decimal? CogsAmount { get; set; }
    public string? CogsFormatted { get; set; }
    public decimal? ProfitAmount { get; set; }
    public string? ProfitFormatted { get; set; }
    public decimal? ProfitMargin { get; set; }
    public string? ProfitMarginFormatted { get; set; }

    // Linked inventory
    public int? InventoryItemId { get; set; }
    public string? InventoryItemTitle { get; set; }
    public string? InventoryItemSku { get; set; }

    // Statuses
    public EbayOrderStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public EbayPaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusDisplay { get; set; } = string.Empty;
    public EbayFulfillmentStatus FulfillmentStatus { get; set; }
    public string FulfillmentStatusDisplay { get; set; } = string.Empty;

    public string? Notes { get; set; }
    public DateTime LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string Currency { get; set; } = "USD";
}

/// <summary>
/// Summary DTO for order listings.
/// </summary>
public class EbayOrderSummaryDto
{
    public int Id { get; set; }
    public string EbayOrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string BuyerUsername { get; set; } = string.Empty;
    public string ItemTitle { get; set; } = string.Empty;
    public string GrossSaleFormatted { get; set; } = string.Empty;
    public string NetPayoutFormatted { get; set; } = string.Empty;
    public string? ProfitFormatted { get; set; }
    public bool IsLinkedToInventory { get; set; }
    public EbayOrderStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
}

/// <summary>
/// DTO for order statistics.
/// </summary>
public class EbayOrderStatsDto
{
    public int TotalOrders { get; set; }
    public int LinkedOrders { get; set; }
    public int UnlinkedOrders { get; set; }
    public decimal TotalGrossSales { get; set; }
    public string TotalGrossSalesFormatted { get; set; } = string.Empty;
    public decimal TotalFees { get; set; }
    public string TotalFeesFormatted { get; set; } = string.Empty;
    public decimal TotalNetPayout { get; set; }
    public string TotalNetPayoutFormatted { get; set; } = string.Empty;
    public decimal TotalProfit { get; set; }
    public string TotalProfitFormatted { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
}
