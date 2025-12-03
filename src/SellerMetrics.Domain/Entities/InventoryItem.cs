using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents an eBay inventory item.
/// </summary>
public class InventoryItem : BaseEntity
{
    /// <summary>
    /// Internal SKU for tracking. Auto-generated if not provided.
    /// </summary>
    public string InternalSku { get; set; } = string.Empty;

    /// <summary>
    /// eBay SKU (custom label). Optional, from eBay listing.
    /// If provided and differs from InternalSku, indicates the item is linked to an eBay listing.
    /// </summary>
    public string? EbaySku { get; set; }

    /// <summary>
    /// Item title/name for identification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Cost of goods sold (what was paid to acquire the item).
    /// This represents the per-unit cost when Quantity > 1.
    /// </summary>
    public Money Cogs { get; set; } = Money.Zero();

    /// <summary>
    /// Quantity of this item in stock.
    /// For items with multiple identical units (e.g., bought a lot of 10).
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Date the item was purchased/acquired.
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// Storage location ID where this item is stored.
    /// </summary>
    public int? StorageLocationId { get; set; }

    /// <summary>
    /// Storage location where this item is stored.
    /// </summary>
    public StorageLocation? StorageLocation { get; set; }

    /// <summary>
    /// Current status of the inventory item.
    /// </summary>
    public InventoryStatus Status { get; set; } = InventoryStatus.Unlisted;

    /// <summary>
    /// Item condition using eBay's condition values.
    /// </summary>
    public EbayCondition? Condition { get; set; }

    /// <summary>
    /// Additional notes about the item.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// File path to the item's photo (for future upload feature).
    /// </summary>
    public string? PhotoPath { get; set; }

    /// <summary>
    /// eBay listing ID when the item is listed.
    /// </summary>
    public string? EbayListingId { get; set; }

    /// <summary>
    /// Date the item was listed on eBay.
    /// </summary>
    public DateTime? ListedDate { get; set; }

    /// <summary>
    /// Date the item was sold.
    /// </summary>
    public DateTime? SoldDate { get; set; }

    /// <summary>
    /// Generates a unique internal SKU.
    /// Format: INV-YYYYMMDD-XXXX (e.g., INV-20250601-0001)
    /// </summary>
    public static string GenerateInternalSku(int sequence)
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{sequence:D4}";
    }

    /// <summary>
    /// Gets the effective SKU (eBay SKU if available, otherwise Internal SKU).
    /// </summary>
    public string EffectiveSku => !string.IsNullOrEmpty(EbaySku) ? EbaySku : InternalSku;

    /// <summary>
    /// Gets the total inventory value (Quantity × COGS).
    /// </summary>
    public Money TotalValue => Cogs.Multiply(Quantity);

    /// <summary>
    /// Sells one unit from this inventory item.
    /// If quantity > 1, decrements quantity and returns a new "Sold" clone with quantity=1.
    /// If quantity = 1, marks this item as sold and returns null.
    /// </summary>
    /// <returns>A new sold InventoryItem if quantity was > 1, otherwise null.</returns>
    public InventoryItem? SellOne()
    {
        if (Quantity <= 0)
        {
            throw new InvalidOperationException("Cannot sell: no quantity available.");
        }

        if (Status == InventoryStatus.Sold)
        {
            throw new InvalidOperationException("Cannot sell: item is already sold.");
        }

        if (Quantity == 1)
        {
            MarkAsSold();
            return null;
        }

        // Quantity > 1: decrement and create a sold clone
        Quantity--;
        UpdatedAt = DateTime.UtcNow;

        return new InventoryItem
        {
            InternalSku = $"{InternalSku}-SOLD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            EbaySku = EbaySku,
            Title = Title,
            Description = Description,
            Cogs = Cogs,
            Quantity = 1,
            PurchaseDate = PurchaseDate,
            StorageLocationId = null, // Sold items don't need storage location
            Status = InventoryStatus.Sold,
            Condition = Condition,
            Notes = Notes,
            PhotoPath = PhotoPath,
            EbayListingId = EbayListingId,
            ListedDate = ListedDate,
            SoldDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marks the item as listed on eBay.
    /// </summary>
    public void MarkAsListed(string? ebayListingId = null)
    {
        Status = InventoryStatus.Listed;
        ListedDate = DateTime.UtcNow;
        EbayListingId = ebayListingId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the item as sold.
    /// </summary>
    public void MarkAsSold()
    {
        Status = InventoryStatus.Sold;
        SoldDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Moves the item to a new storage location.
    /// </summary>
    public void MoveTo(int? storageLocationId)
    {
        StorageLocationId = storageLocationId;
        UpdatedAt = DateTime.UtcNow;
    }
}
