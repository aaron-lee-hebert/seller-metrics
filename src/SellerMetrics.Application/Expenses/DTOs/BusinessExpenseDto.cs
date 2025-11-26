using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Expenses.DTOs;

/// <summary>
/// Data transfer object for BusinessExpense.
/// </summary>
public class BusinessExpenseDto
{
    public int Id { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string AmountFormatted { get; set; } = string.Empty;
    public ExpenseCategory Category { get; set; }
    public string CategoryDisplay { get; set; } = string.Empty;
    public int ScheduleCLine { get; set; }
    public string ScheduleCLineLabel { get; set; } = string.Empty;
    public BusinessLine BusinessLine { get; set; }
    public string BusinessLineDisplay { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? ReceiptPath { get; set; }
    public string? Notes { get; set; }
    public int? ServiceJobId { get; set; }
    public string? ServiceJobNumber { get; set; }
    public bool IsTaxDeductible { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for expense totals by category.
/// </summary>
public class ExpenseByCategoryDto
{
    public ExpenseCategory Category { get; set; }
    public string CategoryDisplay { get; set; } = string.Empty;
    public int ScheduleCLine { get; set; }
    public string ScheduleCLineLabel { get; set; } = string.Empty;
    public string ScheduleCDescription { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalFormatted { get; set; } = string.Empty;
    public int ExpenseCount { get; set; }
}

/// <summary>
/// DTO for expense totals by business line.
/// </summary>
public class ExpenseByBusinessLineDto
{
    public BusinessLine BusinessLine { get; set; }
    public string BusinessLineDisplay { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalFormatted { get; set; } = string.Empty;
    public int ExpenseCount { get; set; }
}

/// <summary>
/// DTO for expense summary.
/// </summary>
public class ExpenseSummaryDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalExpensesFormatted { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public IReadOnlyList<ExpenseByCategoryDto> ByCategory { get; set; } = new List<ExpenseByCategoryDto>();
    public IReadOnlyList<ExpenseByBusinessLineDto> ByBusinessLine { get; set; } = new List<ExpenseByBusinessLineDto>();
}
