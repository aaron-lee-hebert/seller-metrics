namespace SellerMetrics.Domain.Enums;

/// <summary>
/// eBay order status values.
/// </summary>
public enum EbayOrderStatus
{
    /// <summary>
    /// The order is active and in progress.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The order has been completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The order has been cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// The order is inactive.
    /// </summary>
    Inactive = 4
}
