using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.TaxReporting.DTOs;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the reports index/dashboard page.
/// </summary>
public class ReportIndexViewModel
{
    public int SelectedYear { get; set; }
    public List<SelectListItem> YearOptions { get; set; } = new();

    // Quick stats for current year
    public AnnualSummaryDto? CurrentYearSummary { get; set; }

    // Upcoming deadlines
    public List<EstimatedTaxPaymentDto> UpcomingDeadlines { get; set; } = new();
}

/// <summary>
/// ViewModel for quarterly report detail.
/// </summary>
public class QuarterlyReportViewModel
{
    public QuarterlySummaryDto Summary { get; set; } = new();
    public int Year { get; set; }
    public int Quarter { get; set; }
    public List<SelectListItem> YearOptions { get; set; } = new();
    public List<SelectListItem> QuarterOptions { get; set; } = new();
}

/// <summary>
/// ViewModel for annual report detail.
/// </summary>
public class AnnualReportViewModel
{
    public AnnualSummaryDto Summary { get; set; } = new();
    public int Year { get; set; }
    public List<SelectListItem> YearOptions { get; set; } = new();
}
