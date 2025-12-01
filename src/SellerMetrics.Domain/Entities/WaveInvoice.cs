using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents an invoice synced from Wave (read-only).
/// </summary>
public class WaveInvoice : BaseEntity
{
    /// <summary>
    /// The user ID who owns this invoice (foreign key to ApplicationUser).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The unique Wave invoice ID.
    /// </summary>
    public string WaveInvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// The invoice number displayed in Wave.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// The customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The customer's Wave ID.
    /// </summary>
    public string? WaveCustomerId { get; set; }

    /// <summary>
    /// The invoice date.
    /// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// The payment due date.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// The total invoice amount.
    /// </summary>
    public Money TotalAmount { get; set; } = Money.Zero();

    /// <summary>
    /// The amount due (remaining balance).
    /// </summary>
    public Money AmountDue { get; set; } = Money.Zero();

    /// <summary>
    /// The amount paid so far.
    /// </summary>
    public Money AmountPaid { get; set; } = Money.Zero();

    /// <summary>
    /// The invoice status.
    /// </summary>
    public WaveInvoiceStatus Status { get; set; } = WaveInvoiceStatus.Draft;

    /// <summary>
    /// The invoice memo/notes from Wave.
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// The public URL to view the invoice in Wave (for external link).
    /// </summary>
    public string? ViewUrl { get; set; }

    /// <summary>
    /// When this invoice was last synced from Wave (UTC).
    /// </summary>
    public DateTime LastSyncedAt { get; set; }

    /// <summary>
    /// Navigation property to payments for this invoice.
    /// </summary>
    public ICollection<WavePayment> Payments { get; set; } = new List<WavePayment>();

    /// <summary>
    /// Gets whether this invoice is fully paid.
    /// </summary>
    public bool IsPaid => Status == WaveInvoiceStatus.Paid;

    /// <summary>
    /// Gets whether this invoice is overdue.
    /// </summary>
    public bool IsOverdue => !IsPaid && DueDate < DateTime.UtcNow.Date;

    /// <summary>
    /// Updates the invoice from a sync operation.
    /// </summary>
    public void UpdateFromSync(
        WaveInvoiceStatus status,
        Money totalAmount,
        Money amountDue,
        Money amountPaid,
        string? memo,
        string? viewUrl)
    {
        Status = status;
        TotalAmount = totalAmount;
        AmountDue = amountDue;
        AmountPaid = amountPaid;
        Memo = memo;
        ViewUrl = viewUrl;
        LastSyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
