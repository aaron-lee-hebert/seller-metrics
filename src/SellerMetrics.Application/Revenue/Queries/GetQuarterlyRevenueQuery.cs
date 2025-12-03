using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get quarterly revenue for a fiscal year.
/// </summary>
public record GetQuarterlyRevenueQuery(
    int FiscalYear,
    string Currency = "USD");

/// <summary>
/// Handler for GetQuarterlyRevenueQuery.
/// </summary>
public class GetQuarterlyRevenueQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IFiscalYearConfigurationRepository _fiscalYearRepository;

    public GetQuarterlyRevenueQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IFiscalYearConfigurationRepository fiscalYearRepository)
    {
        _revenueRepository = revenueRepository;
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<IReadOnlyList<QuarterlyRevenueDto>> HandleAsync(
        GetQuarterlyRevenueQuery query,
        CancellationToken cancellationToken = default)
    {
        var fiscalConfig = await _fiscalYearRepository.GetActiveAsync(cancellationToken)
            ?? new FiscalYearConfiguration { FiscalYearStartMonth = 1 };

        var result = new List<QuarterlyRevenueDto>();

        for (int quarter = 1; quarter <= 4; quarter++)
        {
            var startDate = fiscalConfig.GetQuarterStart(query.FiscalYear, quarter);
            var endDate = fiscalConfig.GetQuarterEnd(query.FiscalYear, quarter);

            var entries = await _revenueRepository.GetByDateRangeAsync(
                startDate,
                endDate.AddDays(1).AddSeconds(-1), // Include full end date
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

            result.Add(new QuarterlyRevenueDto
            {
                FiscalYear = query.FiscalYear,
                Quarter = quarter,
                QuarterDisplay = $"Q{quarter} {query.FiscalYear}",
                StartDate = startDate,
                EndDate = endDate,
                EbayRevenue = ebayRevenue,
                ServiceRevenue = serviceRevenue,
                TotalRevenue = ebayRevenue + serviceRevenue,
                Currency = query.Currency,
                EbayRevenueFormatted = new Money(ebayRevenue, query.Currency).ToString(),
                ServiceRevenueFormatted = new Money(serviceRevenue, query.Currency).ToString(),
                TotalRevenueFormatted = new Money(ebayRevenue + serviceRevenue, query.Currency).ToString()
            });
        }

        return result;
    }
}
