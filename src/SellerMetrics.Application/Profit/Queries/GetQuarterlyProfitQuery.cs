using SellerMetrics.Application.Profit.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Profit.Queries;

/// <summary>
/// Query to get quarterly profit for a fiscal year.
/// </summary>
public record GetQuarterlyProfitQuery(
    int FiscalYear,
    string Currency = "USD");

/// <summary>
/// Handler for GetQuarterlyProfitQuery.
/// </summary>
public class GetQuarterlyProfitQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IFiscalYearConfigurationRepository _fiscalYearRepository;

    public GetQuarterlyProfitQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IInventoryItemRepository inventoryRepository,
        IFiscalYearConfigurationRepository fiscalYearRepository)
    {
        _revenueRepository = revenueRepository;
        _inventoryRepository = inventoryRepository;
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<IReadOnlyList<QuarterlyProfitDto>> HandleAsync(
        GetQuarterlyProfitQuery query,
        CancellationToken cancellationToken = default)
    {
        var fiscalConfig = await _fiscalYearRepository.GetActiveAsync(cancellationToken)
            ?? new FiscalYearConfiguration { FiscalYearStartMonth = 1 };

        var result = new List<QuarterlyProfitDto>();

        for (int quarter = 1; quarter <= 4; quarter++)
        {
            var startDate = fiscalConfig.GetQuarterStart(query.FiscalYear, quarter);
            var endDate = fiscalConfig.GetQuarterEnd(query.FiscalYear, quarter);

            var entries = await _revenueRepository.GetByDateRangeAsync(
                startDate,
                endDate.AddDays(1).AddSeconds(-1),
                cancellationToken);

            var filteredEntries = entries
                .Where(r => r.GrossAmount.Currency == query.Currency)
                .ToList();

            // Calculate eBay profit
            var ebayEntries = filteredEntries.Where(r => r.Source == RevenueSource.eBay).ToList();
            var ebayRevenue = ebayEntries.Sum(r => r.GrossAmount.Amount - r.Fees.Amount);
            var ebayCogs = 0m;

            foreach (var entry in ebayEntries.Where(e => e.InventoryItemId.HasValue))
            {
                var item = await _inventoryRepository.GetByIdAsync(entry.InventoryItemId!.Value, cancellationToken);
                if (item != null && item.Cogs.Currency == query.Currency)
                {
                    ebayCogs += item.Cogs.Amount;
                }
            }

            var ebayProfit = ebayRevenue - ebayCogs;

            // Calculate service profit
            var serviceEntries = filteredEntries.Where(r => r.Source == RevenueSource.ComputerServices).ToList();
            var serviceProfit = serviceEntries.Sum(r => r.GrossAmount.Amount - r.Fees.Amount);

            result.Add(new QuarterlyProfitDto
            {
                FiscalYear = query.FiscalYear,
                Quarter = quarter,
                QuarterDisplay = $"Q{quarter} {query.FiscalYear}",
                StartDate = startDate,
                EndDate = endDate,
                EbayProfit = ebayProfit,
                ServiceProfit = serviceProfit,
                TotalProfit = ebayProfit + serviceProfit,
                Currency = query.Currency,
                EbayProfitFormatted = new Money(ebayProfit, query.Currency).ToString(),
                ServiceProfitFormatted = new Money(serviceProfit, query.Currency).ToString(),
                TotalProfitFormatted = new Money(ebayProfit + serviceProfit, query.Currency).ToString()
            });
        }

        return result;
    }
}
