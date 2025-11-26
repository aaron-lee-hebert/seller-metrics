namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the Dashboard page.
/// </summary>
public class DashboardViewModel
{
    // Inventory Metrics
    public decimal EbayInventoryValue { get; set; }
    public string EbayInventoryValueFormatted { get; set; } = "$0.00";
    public int EbayInventoryCount { get; set; }
    public decimal ComponentInventoryValue { get; set; }
    public string ComponentInventoryValueFormatted { get; set; } = "$0.00";
    public int ComponentInventoryCount { get; set; }
    public int LowStockComponentCount { get; set; }

    // Quarter-to-Date Metrics
    public decimal QtdRevenue { get; set; }
    public string QtdRevenueFormatted { get; set; } = "$0.00";
    public decimal QtdEbayRevenue { get; set; }
    public string QtdEbayRevenueFormatted { get; set; } = "$0.00";
    public decimal QtdServiceRevenue { get; set; }
    public string QtdServiceRevenueFormatted { get; set; } = "$0.00";
    public decimal QtdProfit { get; set; }
    public string QtdProfitFormatted { get; set; } = "$0.00";
    public bool QtdProfitIsPositive => QtdProfit >= 0;

    // Year-to-Date Metrics
    public decimal YtdRevenue { get; set; }
    public string YtdRevenueFormatted { get; set; } = "$0.00";
    public decimal YtdProfit { get; set; }
    public string YtdProfitFormatted { get; set; } = "$0.00";
    public bool YtdProfitIsPositive => YtdProfit >= 0;

    // Current Period Info
    public int CurrentYear { get; set; }
    public int CurrentQuarter { get; set; }
    public string CurrentQuarterDisplay { get; set; } = string.Empty;

    // Recent Activity
    public List<RecentOrderItem> RecentOrders { get; set; } = new();
    public List<RecentInvoiceItem> RecentInvoices { get; set; } = new();

    // Alerts
    public int UnpaidInvoiceCount { get; set; }
    public decimal UnpaidInvoiceTotal { get; set; }
    public string UnpaidInvoiceTotalFormatted { get; set; } = "$0.00";
    public TaxDeadlineAlert? NextTaxDeadline { get; set; }
    public List<DashboardAlert> Alerts { get; set; } = new();

    // Chart Data
    public MonthlyRevenueChartData ChartData { get; set; } = new();
}

/// <summary>
/// Recent eBay order for dashboard display.
/// </summary>
public class RecentOrderItem
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string ItemTitle { get; set; } = string.Empty;
    public decimal GrossSale { get; set; }
    public string GrossSaleFormatted { get; set; } = "$0.00";
    public decimal Profit { get; set; }
    public string ProfitFormatted { get; set; } = "$0.00";
    public bool ProfitIsPositive => Profit >= 0;
}

/// <summary>
/// Recent service invoice for dashboard display.
/// </summary>
public class RecentInvoiceItem
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string TotalAmountFormatted { get; set; } = "$0.00";
    public string Status { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}

/// <summary>
/// Tax deadline alert for dashboard.
/// </summary>
public class TaxDeadlineAlert
{
    public int TaxYear { get; set; }
    public int Quarter { get; set; }
    public string QuarterDisplay { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string DueDateFormatted { get; set; } = string.Empty;
    public int DaysUntilDue { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsPaid { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string EstimatedAmountFormatted { get; set; } = "$0.00";
}

/// <summary>
/// Generic dashboard alert.
/// </summary>
public class DashboardAlert
{
    public string Type { get; set; } = "info"; // info, warning, danger, success
    public string Icon { get; set; } = "bi-info-circle";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
}

/// <summary>
/// Chart data for monthly revenue trend.
/// </summary>
public class MonthlyRevenueChartData
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> EbayRevenue { get; set; } = new();
    public List<decimal> ServiceRevenue { get; set; } = new();
}
