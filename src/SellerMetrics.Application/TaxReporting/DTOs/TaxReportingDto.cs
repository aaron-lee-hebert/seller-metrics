using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.TaxReporting.DTOs;

/// <summary>
/// DTO for quarterly tax summary report.
/// </summary>
public class QuarterlySummaryDto
{
    /// <summary>
    /// Calendar year (always calendar year for IRS purposes).
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Quarter number (1-4).
    /// </summary>
    public int Quarter { get; set; }

    /// <summary>
    /// Display string (e.g., "Q1 2025").
    /// </summary>
    public string QuarterDisplay { get; set; } = string.Empty;

    /// <summary>
    /// Start date of the quarter.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the quarter.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Revenue from eBay sales.
    /// </summary>
    public decimal EbayRevenue { get; set; }

    /// <summary>
    /// Revenue from computer services.
    /// </summary>
    public decimal ServiceRevenue { get; set; }

    /// <summary>
    /// Total gross revenue (eBay + Services).
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Total fees deduction (eBay fees, payment processing fees).
    /// </summary>
    public decimal TotalFees { get; set; }

    /// <summary>
    /// Total shipping deduction (actual shipping costs paid).
    /// </summary>
    public decimal TotalShipping { get; set; }

    /// <summary>
    /// Revenue breakdown by source.
    /// </summary>
    public IReadOnlyList<RevenueBySourceSummaryDto> RevenueBySource { get; set; } = new List<RevenueBySourceSummaryDto>();

    /// <summary>
    /// Total expenses for the quarter.
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Expenses broken down by Schedule C category.
    /// </summary>
    public IReadOnlyList<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new List<ExpenseByCategoryDto>();

    /// <summary>
    /// Total mileage for the quarter.
    /// </summary>
    public decimal TotalMiles { get; set; }

    /// <summary>
    /// Mileage deduction amount.
    /// </summary>
    public decimal MileageDeduction { get; set; }

    /// <summary>
    /// Mileage deduction breakdown by business line.
    /// </summary>
    public MileageDeductionSummaryDto? MileageBreakdown { get; set; }

    /// <summary>
    /// Net profit (Revenue - Expenses - Mileage Deduction).
    /// </summary>
    public decimal NetProfit { get; set; }

    /// <summary>
    /// Estimated tax payment information for this quarter.
    /// </summary>
    public EstimatedTaxPaymentDto? EstimatedTaxPayment { get; set; }

    // Formatted strings
    public string Currency { get; set; } = "USD";
    public string EbayRevenueFormatted { get; set; } = string.Empty;
    public string ServiceRevenueFormatted { get; set; } = string.Empty;
    public string TotalRevenueFormatted { get; set; } = string.Empty;
    public string TotalFeesFormatted { get; set; } = string.Empty;
    public string TotalShippingFormatted { get; set; } = string.Empty;
    public string TotalExpensesFormatted { get; set; } = string.Empty;
    public string MileageDeductionFormatted { get; set; } = string.Empty;
    public string NetProfitFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for revenue summary by source.
/// </summary>
public class RevenueBySourceSummaryDto
{
    public RevenueSource Source { get; set; }
    public string SourceDisplay { get; set; } = string.Empty;
    public decimal GrossRevenue { get; set; }
    public decimal Fees { get; set; }
    public decimal Shipping { get; set; }
    public decimal NetRevenue { get; set; }
    public int TransactionCount { get; set; }
    public string Currency { get; set; } = "USD";
    public string GrossRevenueFormatted { get; set; } = string.Empty;
    public string FeesFormatted { get; set; } = string.Empty;
    public string ShippingFormatted { get; set; } = string.Empty;
    public string NetRevenueFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for mileage deduction summary.
/// </summary>
public class MileageDeductionSummaryDto
{
    public decimal TotalMiles { get; set; }
    public decimal EbayMiles { get; set; }
    public decimal ServiceMiles { get; set; }
    public decimal SharedMiles { get; set; }
    public decimal ApplicableRate { get; set; }
    public string ApplicableRateFormatted { get; set; } = string.Empty;
    public decimal TotalDeduction { get; set; }
    public decimal EbayDeduction { get; set; }
    public decimal ServiceDeduction { get; set; }
    public decimal SharedDeduction { get; set; }
    public int TripCount { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalDeductionFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for estimated tax payment.
/// </summary>
public class EstimatedTaxPaymentDto
{
    public int Id { get; set; }
    public int TaxYear { get; set; }
    public int Quarter { get; set; }
    public string QuarterDisplay { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string DueDateFormatted { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public string EstimatedAmountFormatted { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public string AmountPaidFormatted { get; set; } = string.Empty;
    public DateTime? PaidDate { get; set; }
    public bool IsPaid { get; set; }
    public bool IsOverdue { get; set; }
    public decimal RemainingAmount { get; set; }
    public string RemainingAmountFormatted { get; set; } = string.Empty;
    public string? ConfirmationNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// DTO for annual tax summary report.
/// </summary>
public class AnnualSummaryDto
{
    /// <summary>
    /// Calendar year for tax reporting.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Total gross revenue for the year.
    /// </summary>
    public decimal TotalGrossRevenue { get; set; }

    /// <summary>
    /// Total fees paid (eBay, payment processing, etc.).
    /// </summary>
    public decimal TotalFees { get; set; }

    /// <summary>
    /// Total shipping costs paid.
    /// </summary>
    public decimal TotalShipping { get; set; }

    /// <summary>
    /// Total net revenue (Gross - Fees - Shipping).
    /// </summary>
    public decimal TotalNetRevenue { get; set; }

    /// <summary>
    /// Revenue from eBay sales.
    /// </summary>
    public decimal EbayRevenue { get; set; }

    /// <summary>
    /// Revenue from computer services.
    /// </summary>
    public decimal ServiceRevenue { get; set; }

    /// <summary>
    /// Total expenses for the year.
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Total mileage deduction.
    /// </summary>
    public decimal TotalMileageDeduction { get; set; }

    /// <summary>
    /// Net profit for the year.
    /// </summary>
    public decimal NetProfit { get; set; }

    /// <summary>
    /// Quarterly breakdown.
    /// </summary>
    public IReadOnlyList<QuarterlySummaryDto> QuarterlyBreakdown { get; set; } = new List<QuarterlySummaryDto>();

    /// <summary>
    /// Schedule C expense summary by line number.
    /// </summary>
    public ScheduleCSummaryDto? ScheduleC { get; set; }

    /// <summary>
    /// Estimated tax payment summary for the year.
    /// </summary>
    public IReadOnlyList<EstimatedTaxPaymentDto> EstimatedTaxPayments { get; set; } = new List<EstimatedTaxPaymentDto>();

    // Formatted strings
    public string Currency { get; set; } = "USD";
    public string TotalGrossRevenueFormatted { get; set; } = string.Empty;
    public string TotalFeesFormatted { get; set; } = string.Empty;
    public string TotalShippingFormatted { get; set; } = string.Empty;
    public string TotalNetRevenueFormatted { get; set; } = string.Empty;
    public string EbayRevenueFormatted { get; set; } = string.Empty;
    public string ServiceRevenueFormatted { get; set; } = string.Empty;
    public string TotalExpensesFormatted { get; set; } = string.Empty;
    public string TotalMileageDeductionFormatted { get; set; } = string.Empty;
    public string NetProfitFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for IRS Schedule C summary matching the form layout.
/// </summary>
public class ScheduleCSummaryDto
{
    public int Year { get; set; }

    // Part I - Income
    /// <summary>
    /// Line 1: Gross receipts or sales.
    /// </summary>
    public decimal Line1_GrossReceipts { get; set; }

    /// <summary>
    /// Line 2: Returns and allowances.
    /// </summary>
    public decimal Line2_ReturnsAndAllowances { get; set; }

    /// <summary>
    /// Line 3: Line 1 minus Line 2 (calculated).
    /// </summary>
    public decimal Line3_NetReceipts => Line1_GrossReceipts - Line2_ReturnsAndAllowances;

    /// <summary>
    /// Line 4: Cost of goods sold (from Part III).
    /// </summary>
    public decimal Line4_CostOfGoodsSold { get; set; }

    /// <summary>
    /// Line 5: Gross profit (Line 3 minus Line 4).
    /// </summary>
    public decimal Line5_GrossProfit => Line3_NetReceipts - Line4_CostOfGoodsSold;

    /// <summary>
    /// Line 7: Gross income (Line 5 + Line 6). Line 6 is other income.
    /// </summary>
    public decimal Line7_GrossIncome { get; set; }

    // Part II - Expenses (matching Schedule C line numbers)
    public IReadOnlyList<ScheduleCLineItemDto> ExpenseLines { get; set; } = new List<ScheduleCLineItemDto>();

    /// <summary>
    /// Line 28: Total expenses before expenses for business use of home.
    /// </summary>
    public decimal Line28_TotalExpenses { get; set; }

    /// <summary>
    /// Line 29: Tentative profit (Line 7 minus Line 28).
    /// </summary>
    public decimal Line29_TentativeProfit => Line7_GrossIncome - Line28_TotalExpenses;

    /// <summary>
    /// Line 31: Net profit or loss.
    /// </summary>
    public decimal Line31_NetProfit { get; set; }

    // Formatted strings
    public string Currency { get; set; } = "USD";
    public string Line1Formatted { get; set; } = string.Empty;
    public string Line3Formatted { get; set; } = string.Empty;
    public string Line4Formatted { get; set; } = string.Empty;
    public string Line5Formatted { get; set; } = string.Empty;
    public string Line7Formatted { get; set; } = string.Empty;
    public string Line28Formatted { get; set; } = string.Empty;
    public string Line29Formatted { get; set; } = string.Empty;
    public string Line31Formatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for a single Schedule C expense line item.
/// </summary>
public class ScheduleCLineItemDto
{
    /// <summary>
    /// Schedule C line number (8-27).
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Line label (e.g., "Line 8").
    /// </summary>
    public string LineLabel { get; set; } = string.Empty;

    /// <summary>
    /// Official IRS description for this line.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Total amount for this line.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Formatted amount string.
    /// </summary>
    public string AmountFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Number of expense entries contributing to this line.
    /// </summary>
    public int ExpenseCount { get; set; }
}
