namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Identifies how the revenue entry was created.
/// </summary>
public enum RevenueEntryType
{
    /// <summary>
    /// Revenue entry was manually created.
    /// </summary>
    Manual = 1,

    /// <summary>
    /// Revenue entry was synced from eBay orders.
    /// </summary>
    EbaySynced = 2,

    /// <summary>
    /// Revenue entry was synced from Wave invoices.
    /// </summary>
    WaveSynced = 3
}
