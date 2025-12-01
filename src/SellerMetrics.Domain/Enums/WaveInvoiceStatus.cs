namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Invoice status values from Wave API.
/// </summary>
public enum WaveInvoiceStatus
{
    /// <summary>
    /// Invoice is in draft state.
    /// </summary>
    Draft,

    /// <summary>
    /// Invoice has been sent to the customer.
    /// </summary>
    Sent,

    /// <summary>
    /// Invoice has been viewed by the customer.
    /// </summary>
    Viewed,

    /// <summary>
    /// Invoice has been partially paid.
    /// </summary>
    PartiallyPaid,

    /// <summary>
    /// Invoice has been fully paid.
    /// </summary>
    Paid,

    /// <summary>
    /// Invoice is overdue.
    /// </summary>
    Overdue,

    /// <summary>
    /// Invoice has been voided/cancelled.
    /// </summary>
    Voided
}
