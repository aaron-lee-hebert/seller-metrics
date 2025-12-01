using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents an eBay order synced from the eBay API.
/// </summary>
public class EbayOrder : BaseEntity
{
    /// <summary>
    /// The user ID who owns this order (foreign key to ApplicationUser).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The unique eBay order ID.
    /// </summary>
    public string EbayOrderId { get; set; } = string.Empty;

    /// <summary>
    /// The legacy eBay order ID (if available).
    /// </summary>
    public string? LegacyOrderId { get; set; }

    /// <summary>
    /// The date and time the order was created on eBay (UTC).
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// The buyer's eBay username.
    /// </summary>
    public string BuyerUsername { get; set; } = string.Empty;

    /// <summary>
    /// The item title from the eBay listing.
    /// </summary>
    public string ItemTitle { get; set; } = string.Empty;

    /// <summary>
    /// The eBay item ID.
    /// </summary>
    public string? EbayItemId { get; set; }

    /// <summary>
    /// The SKU from the eBay listing (for matching to local inventory).
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Quantity sold in this order.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// The gross sale amount (what the buyer paid for the item).
    /// </summary>
    public Money GrossSale { get; set; } = Money.Zero();

    /// <summary>
    /// The shipping amount the buyer paid.
    /// </summary>
    public Money ShippingPaid { get; set; } = Money.Zero();

    /// <summary>
    /// The actual shipping cost you paid (manual entry).
    /// </summary>
    public Money ShippingActual { get; set; } = Money.Zero();

    /// <summary>
    /// eBay final value fee.
    /// </summary>
    public Money FinalValueFee { get; set; } = Money.Zero();

    /// <summary>
    /// Payment processing fee (e.g., PayPal, eBay Managed Payments).
    /// </summary>
    public Money PaymentProcessingFee { get; set; } = Money.Zero();

    /// <summary>
    /// Any additional eBay fees (promoted listings, international fees, etc.).
    /// </summary>
    public Money AdditionalFees { get; set; } = Money.Zero();

    /// <summary>
    /// The linked inventory item (for COGS calculation).
    /// </summary>
    public int? InventoryItemId { get; set; }

    /// <summary>
    /// Navigation property to the linked inventory item.
    /// </summary>
    public InventoryItem? InventoryItem { get; set; }

    /// <summary>
    /// The order status from eBay.
    /// </summary>
    public EbayOrderStatus Status { get; set; } = EbayOrderStatus.Active;

    /// <summary>
    /// The payment status from eBay.
    /// </summary>
    public EbayPaymentStatus PaymentStatus { get; set; } = EbayPaymentStatus.Pending;

    /// <summary>
    /// The fulfillment status from eBay.
    /// </summary>
    public EbayFulfillmentStatus FulfillmentStatus { get; set; } = EbayFulfillmentStatus.NotStarted;

    /// <summary>
    /// Notes about the order (user-entered).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When this order was last synced from eBay (UTC).
    /// </summary>
    public DateTime LastSyncedAt { get; set; }

    /// <summary>
    /// Gets the total fees (Final Value + Payment Processing + Additional).
    /// </summary>
    public Money TotalFees => FinalValueFee + PaymentProcessingFee + AdditionalFees;

    /// <summary>
    /// Gets the net payout (Gross Sale + Shipping Paid - Total Fees).
    /// This is what eBay deposits to your account.
    /// </summary>
    public Money NetPayout => GrossSale + ShippingPaid - TotalFees;

    /// <summary>
    /// Gets the COGS from the linked inventory item (or zero if not linked).
    /// </summary>
    public Money Cogs => InventoryItem?.Cogs ?? Money.Zero();

    /// <summary>
    /// Gets the calculated profit (Net Payout - COGS - Actual Shipping Cost).
    /// Returns null if no inventory item is linked.
    /// </summary>
    public Money? Profit
    {
        get
        {
            if (InventoryItem == null)
                return null;

            return NetPayout - Cogs - ShippingActual;
        }
    }

    /// <summary>
    /// Gets the profit margin as a percentage (Profit / Gross Sale * 100).
    /// Returns null if profit cannot be calculated or gross sale is zero.
    /// </summary>
    public decimal? ProfitMargin
    {
        get
        {
            var profit = Profit;
            if (profit == null || GrossSale.IsZero)
                return null;

            return (profit.Amount / GrossSale.Amount) * 100;
        }
    }

    /// <summary>
    /// Updates the actual shipping cost.
    /// </summary>
    /// <param name="actualShippingCost">The actual shipping cost paid.</param>
    public void UpdateShippingCost(Money actualShippingCost)
    {
        ShippingActual = actualShippingCost;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Links this order to an inventory item.
    /// </summary>
    /// <param name="inventoryItemId">The inventory item ID.</param>
    public void LinkToInventory(int inventoryItemId)
    {
        InventoryItemId = inventoryItemId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unlinks this order from inventory.
    /// </summary>
    public void UnlinkFromInventory()
    {
        InventoryItemId = null;
        InventoryItem = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the order from a sync operation.
    /// </summary>
    public void UpdateFromSync(
        EbayOrderStatus status,
        EbayPaymentStatus paymentStatus,
        EbayFulfillmentStatus fulfillmentStatus,
        Money? finalValueFee = null,
        Money? paymentProcessingFee = null,
        Money? additionalFees = null)
    {
        Status = status;
        PaymentStatus = paymentStatus;
        FulfillmentStatus = fulfillmentStatus;

        if (finalValueFee != null)
            FinalValueFee = finalValueFee;

        if (paymentProcessingFee != null)
            PaymentProcessingFee = paymentProcessingFee;

        if (additionalFees != null)
            AdditionalFees = additionalFees;

        LastSyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
