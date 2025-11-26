using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellerMetrics.Application.Inventory.Queries;
using SellerMetrics.Application.Components.Queries;
using SellerMetrics.Application.Revenue.Queries;
using SellerMetrics.Application.Profit.Queries;
using SellerMetrics.Application.TaxReporting.Queries;
using SellerMetrics.Domain.ValueObjects;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GetInventoryValueQueryHandler _inventoryValueHandler;
    private readonly GetComponentValueQueryHandler _componentValueHandler;
    private readonly GetLowStockComponentsQueryHandler _lowStockHandler;
    private readonly GetQuarterlyRevenueQueryHandler _quarterlyRevenueHandler;
    private readonly GetYearToDateRevenueQueryHandler _ytdRevenueHandler;
    private readonly GetQuarterlyProfitQueryHandler _quarterlyProfitHandler;
    private readonly GetQuarterlySummaryQueryHandler _quarterlySummaryHandler;
    private readonly GetMonthlyRevenueQueryHandler _monthlyRevenueHandler;

    public HomeController(
        ILogger<HomeController> logger,
        GetInventoryValueQueryHandler inventoryValueHandler,
        GetComponentValueQueryHandler componentValueHandler,
        GetLowStockComponentsQueryHandler lowStockHandler,
        GetQuarterlyRevenueQueryHandler quarterlyRevenueHandler,
        GetYearToDateRevenueQueryHandler ytdRevenueHandler,
        GetQuarterlyProfitQueryHandler quarterlyProfitHandler,
        GetQuarterlySummaryQueryHandler quarterlySummaryHandler,
        GetMonthlyRevenueQueryHandler monthlyRevenueHandler)
    {
        _logger = logger;
        _inventoryValueHandler = inventoryValueHandler;
        _componentValueHandler = componentValueHandler;
        _lowStockHandler = lowStockHandler;
        _quarterlyRevenueHandler = quarterlyRevenueHandler;
        _ytdRevenueHandler = ytdRevenueHandler;
        _quarterlyProfitHandler = quarterlyProfitHandler;
        _quarterlySummaryHandler = quarterlySummaryHandler;
        _monthlyRevenueHandler = monthlyRevenueHandler;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var currentYear = today.Year;
        var currentQuarter = (today.Month - 1) / 3 + 1;

        var viewModel = new DashboardViewModel
        {
            CurrentYear = currentYear,
            CurrentQuarter = currentQuarter,
            CurrentQuarterDisplay = $"Q{currentQuarter} {currentYear}"
        };

        try
        {
            // Get inventory values
            var inventoryValue = await _inventoryValueHandler.HandleAsync(
                new GetInventoryValueQuery(), CancellationToken.None);
            viewModel.EbayInventoryValue = inventoryValue.TotalValue;
            viewModel.EbayInventoryValueFormatted = inventoryValue.TotalValueFormatted;
            viewModel.EbayInventoryCount = inventoryValue.TotalItems;

            // Get component values
            var componentValue = await _componentValueHandler.HandleAsync(
                new GetComponentValueQuery(), CancellationToken.None);
            viewModel.ComponentInventoryValue = componentValue.TotalValue;
            viewModel.ComponentInventoryValueFormatted = componentValue.TotalValueFormatted;
            viewModel.ComponentInventoryCount = componentValue.TotalItems;

            // Get low stock components count
            var lowStockComponents = await _lowStockHandler.HandleAsync(
                new GetLowStockComponentsQuery(), CancellationToken.None);
            viewModel.LowStockComponentCount = lowStockComponents.Count;

            // Get quarterly revenue (returns all quarters for the year)
            var quarterlyRevenues = await _quarterlyRevenueHandler.HandleAsync(
                new GetQuarterlyRevenueQuery(currentYear), CancellationToken.None);
            var qtdRevenue = quarterlyRevenues.FirstOrDefault(q => q.Quarter == currentQuarter);
            if (qtdRevenue != null)
            {
                viewModel.QtdRevenue = qtdRevenue.TotalRevenue;
                viewModel.QtdRevenueFormatted = qtdRevenue.TotalRevenueFormatted;
                viewModel.QtdEbayRevenue = qtdRevenue.EbayRevenue;
                viewModel.QtdEbayRevenueFormatted = qtdRevenue.EbayRevenueFormatted;
                viewModel.QtdServiceRevenue = qtdRevenue.ServiceRevenue;
                viewModel.QtdServiceRevenueFormatted = qtdRevenue.ServiceRevenueFormatted;
            }

            // Get quarterly profit (returns all quarters for the year)
            var quarterlyProfits = await _quarterlyProfitHandler.HandleAsync(
                new GetQuarterlyProfitQuery(currentYear), CancellationToken.None);
            var qtdProfit = quarterlyProfits.FirstOrDefault(q => q.Quarter == currentQuarter);
            if (qtdProfit != null)
            {
                viewModel.QtdProfit = qtdProfit.TotalProfit;
                viewModel.QtdProfitFormatted = qtdProfit.TotalProfitFormatted;
            }

            // Get year-to-date revenue
            var ytdRevenue = await _ytdRevenueHandler.HandleAsync(
                new GetYearToDateRevenueQuery(currentYear), CancellationToken.None);
            viewModel.YtdRevenue = ytdRevenue.TotalRevenue;
            viewModel.YtdRevenueFormatted = ytdRevenue.TotalRevenueFormatted;

            // Calculate YTD profit (sum of completed quarters)
            decimal ytdProfit = quarterlyProfits
                .Where(q => q.Quarter <= currentQuarter)
                .Sum(q => q.TotalProfit);
            viewModel.YtdProfit = ytdProfit;
            viewModel.YtdProfitFormatted = new Money(ytdProfit, "USD").ToString();

            // Get monthly revenue for chart (last 6 months)
            var chartData = await GetMonthlyChartDataAsync(6);
            viewModel.ChartData = chartData;

            // Build alerts
            viewModel.Alerts = BuildAlerts(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            // Continue with default values - dashboard should still render
        }

        ViewData["Title"] = "Dashboard";
        return View(viewModel);
    }

    private async Task<MonthlyRevenueChartData> GetMonthlyChartDataAsync(int months)
    {
        var chartData = new MonthlyRevenueChartData();
        var today = DateTime.Today;

        for (int i = months - 1; i >= 0; i--)
        {
            var targetDate = today.AddMonths(-i);
            var startDate = new DateTime(targetDate.Year, targetDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            chartData.Labels.Add(targetDate.ToString("MMM yyyy"));

            try
            {
                var monthlyRevenueList = await _monthlyRevenueHandler.HandleAsync(
                    new GetMonthlyRevenueQuery(startDate, endDate), CancellationToken.None);

                // The handler returns a list - get the first (should be only one for single month range)
                var monthlyRevenue = monthlyRevenueList.FirstOrDefault();
                chartData.EbayRevenue.Add(monthlyRevenue?.EbayRevenue ?? 0);
                chartData.ServiceRevenue.Add(monthlyRevenue?.ServiceRevenue ?? 0);
            }
            catch
            {
                chartData.EbayRevenue.Add(0);
                chartData.ServiceRevenue.Add(0);
            }
        }

        return chartData;
    }

    private List<DashboardAlert> BuildAlerts(DashboardViewModel model)
    {
        var alerts = new List<DashboardAlert>();

        // Low stock alert
        if (model.LowStockComponentCount > 0)
        {
            alerts.Add(new DashboardAlert
            {
                Type = "warning",
                Icon = "bi-exclamation-triangle",
                Title = "Low Stock",
                Message = $"{model.LowStockComponentCount} component(s) are running low on stock.",
                ActionUrl = Url.Action("Index", "Component", new { filter = "lowstock" }),
                ActionText = "View Components"
            });
        }

        // Negative profit alert
        if (model.QtdProfit < 0)
        {
            alerts.Add(new DashboardAlert
            {
                Type = "danger",
                Icon = "bi-graph-down-arrow",
                Title = "Negative Profit",
                Message = $"Quarter-to-date profit is negative ({model.QtdProfitFormatted}).",
                ActionUrl = Url.Action("Quarterly", "Report"),
                ActionText = "View Report"
            });
        }

        return alerts;
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
