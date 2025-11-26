using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents an estimated quarterly tax payment for IRS Form 1040-ES.
/// </summary>
public class EstimatedTaxPayment
{
    public int Id { get; set; }

    /// <summary>
    /// Calendar year for the tax payment (always calendar year for IRS purposes).
    /// </summary>
    public int TaxYear { get; set; }

    /// <summary>
    /// Quarter number (1-4).
    /// Q1 = Jan-Mar (due Apr 15)
    /// Q2 = Apr-May (due Jun 15)
    /// Q3 = Jun-Aug (due Sep 15)
    /// Q4 = Sep-Dec (due Jan 15 of following year)
    /// </summary>
    public int Quarter { get; set; }

    /// <summary>
    /// The IRS due date for this payment.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// The estimated amount owed for this quarter.
    /// </summary>
    public Money EstimatedAmount { get; set; } = Money.Zero();

    /// <summary>
    /// The actual amount paid.
    /// </summary>
    public Money AmountPaid { get; set; } = Money.Zero();

    /// <summary>
    /// Date the payment was made.
    /// </summary>
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// Whether the payment has been made.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Confirmation number from IRS or payment processor.
    /// </summary>
    public string? ConfirmationNumber { get; set; }

    /// <summary>
    /// Payment method (e.g., "IRS Direct Pay", "EFTPS", "Check").
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Notes about the payment.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date the record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the quarter display string (e.g., "Q1 2025").
    /// </summary>
    public string QuarterDisplay => $"Q{Quarter} {TaxYear}";

    /// <summary>
    /// Gets whether this payment is overdue.
    /// </summary>
    public bool IsOverdue => !IsPaid && DateTime.Today > DueDate;

    /// <summary>
    /// Gets the remaining amount to be paid.
    /// </summary>
    public decimal RemainingAmount => Math.Max(0, EstimatedAmount.Amount - AmountPaid.Amount);

    /// <summary>
    /// Calculates the IRS due date for a given quarter and year.
    /// </summary>
    public static DateTime GetDueDate(int year, int quarter)
    {
        return quarter switch
        {
            1 => new DateTime(year, 4, 15),           // Q1: April 15
            2 => new DateTime(year, 6, 15),           // Q2: June 15
            3 => new DateTime(year, 9, 15),           // Q3: September 15
            4 => new DateTime(year + 1, 1, 15),       // Q4: January 15 of next year
            _ => throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be 1-4")
        };
    }

    /// <summary>
    /// Gets the date range for a quarter (calendar year basis for tax purposes).
    /// </summary>
    public static (DateTime Start, DateTime End) GetQuarterDateRange(int year, int quarter)
    {
        return quarter switch
        {
            1 => (new DateTime(year, 1, 1), new DateTime(year, 3, 31)),
            2 => (new DateTime(year, 4, 1), new DateTime(year, 5, 31)),  // IRS Q2 is only Apr-May
            3 => (new DateTime(year, 6, 1), new DateTime(year, 8, 31)),  // IRS Q3 is Jun-Aug
            4 => (new DateTime(year, 9, 1), new DateTime(year, 12, 31)), // IRS Q4 is Sep-Dec
            _ => throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be 1-4")
        };
    }
}
