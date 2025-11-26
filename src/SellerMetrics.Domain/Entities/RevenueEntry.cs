using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a revenue entry from either eBay sales or computer services.
/// Supports both synced and manual entry.
/// </summary>
public class RevenueEntry : BaseEntity
{
    /// <summary>
    /// The source of this revenue (eBay or ComputerServices).
    /// </summary>
    public RevenueSource Source { get; set; }

    /// <summary>
    /// How this entry was created (Manual, EbaySynced, WaveSynced).
    /// </summary>
    public RevenueEntryType EntryType { get; set; } = RevenueEntryType.Manual;

    /// <summary>
    /// The date the revenue was earned/received.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Description of the revenue source (e.g., order details, service description).
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gross amount before fees and expenses.
    /// </summary>
    public Money GrossAmount { get; set; } = Money.Zero();

    /// <summary>
    /// Fees deducted (eBay fees, payment processing, etc.).
    /// </summary>
    public Money Fees { get; set; } = Money.Zero();

    /// <summary>
    /// Net amount after fees (GrossAmount - Fees).
    /// </summary>
    public Money NetAmount => GrossAmount - Fees;

    /// <summary>
    /// eBay order ID for synced eBay orders.
    /// Used to link and prevent duplicate imports.
    /// </summary>
    public string? EbayOrderId { get; set; }

    /// <summary>
    /// Wave invoice number for service revenue.
    /// Used to link and prevent duplicate imports.
    /// </summary>
    public string? WaveInvoiceNumber { get; set; }

    /// <summary>
    /// Link to the inventory item if this revenue is from selling inventory.
    /// </summary>
    public int? InventoryItemId { get; set; }

    /// <summary>
    /// Navigation property to the inventory item.
    /// </summary>
    public InventoryItem? InventoryItem { get; set; }

    /// <summary>
    /// Link to the service job if this revenue is from a service.
    /// </summary>
    public int? ServiceJobId { get; set; }

    /// <summary>
    /// Navigation property to the service job.
    /// </summary>
    public ServiceJob? ServiceJob { get; set; }

    /// <summary>
    /// Additional notes about this revenue entry.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Timestamp of last sync from external source (eBay/Wave).
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }
}
