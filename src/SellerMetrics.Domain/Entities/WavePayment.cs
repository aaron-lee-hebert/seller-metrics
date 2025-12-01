using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a payment on a Wave invoice (read-only).
/// </summary>
public class WavePayment : BaseEntity
{
    /// <summary>
    /// The user ID who owns this payment (foreign key to ApplicationUser).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The unique Wave payment ID.
    /// </summary>
    public string WavePaymentId { get; set; } = string.Empty;

    /// <summary>
    /// The Wave invoice ID this payment is for.
    /// </summary>
    public int WaveInvoiceId { get; set; }

    /// <summary>
    /// Navigation property to the invoice.
    /// </summary>
    public WaveInvoice? Invoice { get; set; }

    /// <summary>
    /// The payment date.
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// The payment amount.
    /// </summary>
    public Money Amount { get; set; } = Money.Zero();

    /// <summary>
    /// The payment method/source (e.g., "Credit Card", "Bank Transfer", "Cash").
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Notes about the payment from Wave.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When this payment was last synced from Wave (UTC).
    /// </summary>
    public DateTime LastSyncedAt { get; set; }

    /// <summary>
    /// Updates the payment from a sync operation.
    /// </summary>
    public void UpdateFromSync(
        Money amount,
        string? paymentMethod,
        string? notes)
    {
        Amount = amount;
        PaymentMethod = paymentMethod;
        Notes = notes;
        LastSyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
