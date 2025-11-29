using System.Globalization;
using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get year-to-date revenue summary.
/// </summary>
public record GetYearToDateRevenueQuery(
    int? FiscalYear = null,
    string Currency = "USD");

/// <summary>
/// Handler for GetYearToDateRevenueQuery.
/// </summary>
public class GetYearToDateRevenueQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IFiscalYearConfigurationRepository _fiscalYearRepository;

    public GetYearToDateRevenueQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IFiscalYearConfigurationRepository fiscalYearRepository)
    {
        _revenueRepository = revenueRepository;
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<YearToDateRevenueDto> HandleAsync(
        GetYearToDateRevenueQuery query,
        CancellationToken cancellationToken = default)
    {
        var fiscalConfig = await _fiscalYearRepository.GetActiveAsync(cancellationToken)
            ?? new FiscalYearConfiguration { FiscalYearStartMonth = 1 };

        var today = DateTime.UtcNow;
        var fiscalYear = query.FiscalYear ?? fiscalConfig.GetFiscalYear(today);

        var startDate = fiscalConfig.GetFiscalYearStart(fiscalYear);
        var endDate = today < fiscalConfig.GetFiscalYearEnd(fiscalYear)
            ? today
            : fiscalConfig.GetFiscalYearEnd(fiscalYear);

        var entries = await _revenueRepository.GetByDateRangeAsync(
            startDate,
            endDate,
            cancellationToken);

        var filteredEntries = entries
            .Where(e => e.GrossAmount.Currency == query.Currency)
            .ToList();

        var ebayRevenue = filteredEntries
            .Where(e => e.Source == RevenueSource.eBay)
            .Sum(e => e.NetAmount.Amount);

        var serviceRevenue = filteredEntries
            .Where(e => e.Source == RevenueSource.ComputerServices)
            .Sum(e => e.NetAmount.Amount);

        // Calculate monthly breakdown
        var monthlyBreakdown = filteredEntries
            .GroupBy(e => new { e.TransactionDate.Year, e.TransactionDate.Month })
            .Select(g =>
            {
                var monthEbay = g
                    .Where(e => e.Source == RevenueSource.eBay)
                    .Sum(e => e.NetAmount.Amount);

                var monthService = g
                    .Where(e => e.Source == RevenueSource.ComputerServices)
                    .Sum(e => e.NetAmount.Amount);

                return new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    EbayRevenue = monthEbay,
                    ServiceRevenue = monthService,
                    TotalRevenue = monthEbay + monthService,
                    Currency = query.Currency,
                    EbayRevenueFormatted = new Money(monthEbay, query.Currency).ToString(),
                    ServiceRevenueFormatted = new Money(monthService, query.Currency).ToString(),
                    TotalRevenueFormatted = new Money(monthEbay + monthService, query.Currency).ToString()
                };
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        return new YearToDateRevenueDto
        {
            FiscalYear = fiscalYear,
            StartDate = startDate,
            EndDate = endDate,
            EbayRevenue = ebayRevenue,
            ServiceRevenue = serviceRevenue,
            TotalRevenue = ebayRevenue + serviceRevenue,
            Currency = query.Currency,
            EbayRevenueFormatted = new Money(ebayRevenue, query.Currency).ToString(),
            ServiceRevenueFormatted = new Money(serviceRevenue, query.Currency).ToString(),
            TotalRevenueFormatted = new Money(ebayRevenue + serviceRevenue, query.Currency).ToString(),
            MonthlyBreakdown = monthlyBreakdown
        };
    }
}
