using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Profit.DTOs;

/// <summary>
/// DTO for profit summary by source.
/// </summary>
public class ProfitBySourceDto
{
    public RevenueSource Source { get; set; }
    public string SourceDisplay { get; set; } = string.Empty;
    public decimal GrossRevenue { get; set; }
    public decimal Fees { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal Cogs { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
    public string Currency { get; set; } = "USD";
    public string GrossRevenueFormatted { get; set; } = string.Empty;
    public string FeesFormatted { get; set; } = string.Empty;
    public string NetRevenueFormatted { get; set; } = string.Empty;
    public string CogsFormatted { get; set; } = string.Empty;
    public string ExpensesFormatted { get; set; } = string.Empty;
    public string ProfitFormatted { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO for combined profit summary.
/// </summary>
public class CombinedProfitDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalGrossRevenue { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalNetRevenue { get; set; }
    public decimal TotalCogs { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal OverallProfitMargin { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalGrossRevenueFormatted { get; set; } = string.Empty;
    public string TotalFeesFormatted { get; set; } = string.Empty;
    public string TotalNetRevenueFormatted { get; set; } = string.Empty;
    public string TotalCogsFormatted { get; set; } = string.Empty;
    public string TotalExpensesFormatted { get; set; } = string.Empty;
    public string TotalProfitFormatted { get; set; } = string.Empty;
    public ProfitBySourceDto? EbayProfit { get; set; }
    public ProfitBySourceDto? ServiceProfit { get; set; }
}

/// <summary>
/// DTO for monthly profit breakdown.
/// </summary>
public class MonthlyProfitDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal EbayProfit { get; set; }
    public decimal ServiceProfit { get; set; }
    public decimal TotalProfit { get; set; }
    public string Currency { get; set; } = "USD";
    public string EbayProfitFormatted { get; set; } = string.Empty;
    public string ServiceProfitFormatted { get; set; } = string.Empty;
    public string TotalProfitFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for quarterly profit summary.
/// </summary>
public class QuarterlyProfitDto
{
    public int FiscalYear { get; set; }
    public int Quarter { get; set; }
    public string QuarterDisplay { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EbayProfit { get; set; }
    public decimal ServiceProfit { get; set; }
    public decimal TotalProfit { get; set; }
    public string Currency { get; set; } = "USD";
    public string EbayProfitFormatted { get; set; } = string.Empty;
    public string ServiceProfitFormatted { get; set; } = string.Empty;
    public string TotalProfitFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for service job profit calculation.
/// </summary>
public class ServiceJobProfitDto
{
    public int ServiceJobId { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal ComponentCosts { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
    public string Currency { get; set; } = "USD";
    public string RevenueFormatted { get; set; } = string.Empty;
    public string ComponentCostsFormatted { get; set; } = string.Empty;
    public string ExpensesFormatted { get; set; } = string.Empty;
    public string ProfitFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for tax reporting profit summary.
/// </summary>
public class TaxReportProfitDto
{
    public int FiscalYear { get; set; }
    public decimal TotalGrossReceipts { get; set; }
    public decimal TotalReturnsAndAllowances { get; set; }
    public decimal NetGrossReceipts { get; set; }
    public decimal CostOfGoodsSold { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalGrossReceiptsFormatted { get; set; } = string.Empty;
    public string CostOfGoodsSoldFormatted { get; set; } = string.Empty;
    public string GrossProfitFormatted { get; set; } = string.Empty;
    public string TotalExpensesFormatted { get; set; } = string.Empty;
    public string NetProfitFormatted { get; set; } = string.Empty;
    public IReadOnlyList<QuarterlyProfitDto> QuarterlyBreakdown { get; set; } = new List<QuarterlyProfitDto>();
}
