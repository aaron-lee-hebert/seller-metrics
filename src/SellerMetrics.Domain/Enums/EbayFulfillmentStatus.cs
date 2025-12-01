namespace SellerMetrics.Domain.Enums;

/// <summary>
/// eBay fulfillment/shipping status values.
/// </summary>
public enum EbayFulfillmentStatus
{
    /// <summary>
    /// Fulfillment has not started.
    /// </summary>
    NotStarted = 1,

    /// <summary>
    /// Fulfillment is in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Order has been shipped/fulfilled.
    /// </summary>
    Fulfilled = 3
}
