namespace SellerMetrics.Domain.Enums;

/// <summary>
/// eBay payment status values.
/// </summary>
public enum EbayPaymentStatus
{
    /// <summary>
    /// Payment is pending.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment has failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Payment is fully paid.
    /// </summary>
    Paid = 3,

    /// <summary>
    /// Payment is partially refunded.
    /// </summary>
    PartiallyRefunded = 4,

    /// <summary>
    /// Payment is fully refunded.
    /// </summary>
    FullyRefunded = 5
}
