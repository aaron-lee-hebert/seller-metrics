using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a business expense for tax reporting.
/// </summary>
public class BusinessExpense : BaseEntity
{
    /// <summary>
    /// Date the expense was incurred.
    /// </summary>
    public DateTime ExpenseDate { get; set; }

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The expense amount.
    /// </summary>
    public Money Amount { get; set; } = Money.Zero();

    /// <summary>
    /// IRS Schedule C category for this expense.
    /// </summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>
    /// Business line this expense is associated with.
    /// </summary>
    public BusinessLine BusinessLine { get; set; }

    /// <summary>
    /// Optional vendor or payee name.
    /// </summary>
    public string? Vendor { get; set; }

    /// <summary>
    /// Path to receipt image/document (for future upload feature).
    /// </summary>
    public string? ReceiptPath { get; set; }

    /// <summary>
    /// Additional notes about the expense.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Optional link to a specific service job (for service-related expenses).
    /// </summary>
    public int? ServiceJobId { get; set; }

    /// <summary>
    /// Navigation property to the linked service job.
    /// </summary>
    public ServiceJob? ServiceJob { get; set; }

    /// <summary>
    /// Whether this expense is tax deductible.
    /// Defaults to true for all business expenses.
    /// </summary>
    public bool IsTaxDeductible { get; set; } = true;

    /// <summary>
    /// Optional reference number (invoice #, receipt #, etc.).
    /// </summary>
    public string? ReferenceNumber { get; set; }
}
