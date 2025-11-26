using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.TaxReporting.Commands;
using SellerMetrics.Application.TaxReporting.DTOs;
using SellerMetrics.Application.TaxReporting.Queries;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for quarterly and annual tax reports.
/// </summary>
[Authorize]
public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger;
    private readonly GetQuarterlySummaryQueryHandler _quarterlyHandler;
    private readonly GetAnnualSummaryQueryHandler _annualHandler;
    private readonly ExportTaxReportCommandHandler _exportHandler;

    public ReportController(
        ILogger<ReportController> logger,
        GetQuarterlySummaryQueryHandler quarterlyHandler,
        GetAnnualSummaryQueryHandler annualHandler,
        ExportTaxReportCommandHandler exportHandler)
    {
        _logger = logger;
        _quarterlyHandler = quarterlyHandler;
        _annualHandler = annualHandler;
        _exportHandler = exportHandler;
    }

    public async Task<IActionResult> Index(int? year)
    {
        var selectedYear = year ?? DateTime.Today.Year;

        var viewModel = new ReportIndexViewModel
        {
            SelectedYear = selectedYear,
            YearOptions = GetYearOptions(selectedYear)
        };

        try
        {
            // Get current year summary
            viewModel.CurrentYearSummary = await _annualHandler.HandleAsync(
                new GetAnnualSummaryQuery(selectedYear),
                CancellationToken.None);

            // Get upcoming tax payment deadlines
            viewModel.UpcomingDeadlines = viewModel.CurrentYearSummary.EstimatedTaxPayments
                .Where(p => !p.IsPaid && p.DueDate >= DateTime.Today)
                .OrderBy(p => p.DueDate)
                .Take(4)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports dashboard for year {Year}", selectedYear);
            TempData["ErrorMessage"] = "Failed to load reports.";
        }

        ViewData["Title"] = "Reports";
        return View(viewModel);
    }

    public async Task<IActionResult> Quarterly(int? year, int? quarter)
    {
        var today = DateTime.Today;
        var selectedYear = year ?? today.Year;
        var selectedQuarter = quarter ?? GetCurrentQuarter(today);

        var viewModel = new QuarterlyReportViewModel
        {
            Year = selectedYear,
            Quarter = selectedQuarter,
            YearOptions = GetYearOptions(selectedYear),
            QuarterOptions = GetQuarterOptions(selectedQuarter)
        };

        try
        {
            viewModel.Summary = await _quarterlyHandler.HandleAsync(
                new GetQuarterlySummaryQuery(selectedYear, selectedQuarter),
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading quarterly report for {Year} Q{Quarter}", selectedYear, selectedQuarter);
            TempData["ErrorMessage"] = "Failed to load quarterly report.";
        }

        ViewData["Title"] = $"Q{selectedQuarter} {selectedYear} Report";
        ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Reports</a></li><li class=\"breadcrumb-item active\">Q{selectedQuarter} {selectedYear}</li>";
        return View(viewModel);
    }

    public async Task<IActionResult> Annual(int? year)
    {
        var selectedYear = year ?? DateTime.Today.Year;

        var viewModel = new AnnualReportViewModel
        {
            Year = selectedYear,
            YearOptions = GetYearOptions(selectedYear)
        };

        try
        {
            viewModel.Summary = await _annualHandler.HandleAsync(
                new GetAnnualSummaryQuery(selectedYear),
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading annual report for {Year}", selectedYear);
            TempData["ErrorMessage"] = "Failed to load annual report.";
        }

        ViewData["Title"] = $"{selectedYear} Annual Report";
        ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Reports</a></li><li class=\"breadcrumb-item active\">{selectedYear}</li>";
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Export(int year, string format = "excel")
    {
        try
        {
            var exportFormat = format.ToLowerInvariant() switch
            {
                "csv" => ExportFormat.Csv,
                _ => ExportFormat.Excel
            };

            var result = await _exportHandler.HandleAsync(
                new ExportTaxReportCommand(year, exportFormat),
                CancellationToken.None);

            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting tax report for {Year}", year);
            TempData["ErrorMessage"] = "Failed to export report.";
            return RedirectToAction(nameof(Annual), new { year });
        }
    }

    #region Helper Methods

    private static int GetCurrentQuarter(DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    private static List<SelectListItem> GetYearOptions(int selectedYear)
    {
        var currentYear = DateTime.Today.Year;
        var years = new List<SelectListItem>();

        // Include years from 2020 to current year + 1
        for (int y = currentYear + 1; y >= 2020; y--)
        {
            years.Add(new SelectListItem
            {
                Value = y.ToString(),
                Text = y.ToString(),
                Selected = y == selectedYear
            });
        }

        return years;
    }

    private static List<SelectListItem> GetQuarterOptions(int selectedQuarter)
    {
        return new List<SelectListItem>
        {
            new() { Value = "1", Text = "Q1 (Jan - Mar)", Selected = selectedQuarter == 1 },
            new() { Value = "2", Text = "Q2 (Apr - Jun)", Selected = selectedQuarter == 2 },
            new() { Value = "3", Text = "Q3 (Jul - Sep)", Selected = selectedQuarter == 3 },
            new() { Value = "4", Text = "Q4 (Oct - Dec)", Selected = selectedQuarter == 4 }
        };
    }

    #endregion
}
