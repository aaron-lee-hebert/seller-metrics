namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Status of an inventory item in the system.
/// </summary>
public enum InventoryStatus
{
    /// <summary>
    /// Item has been added to inventory but not yet listed on eBay.
    /// </summary>
    Unlisted = 0,

    /// <summary>
    /// Item is currently listed for sale on eBay.
    /// </summary>
    Listed = 1,

    /// <summary>
    /// Item has been sold and is awaiting shipment or has been shipped.
    /// </summary>
    Sold = 2
}
