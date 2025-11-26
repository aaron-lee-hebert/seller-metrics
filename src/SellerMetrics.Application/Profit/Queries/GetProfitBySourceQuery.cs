using SellerMetrics.Application.Profit.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Profit.Queries;

/// <summary>
/// Query to get profit breakdown by source for a date range.
/// </summary>
public record GetProfitBySourceQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetProfitBySourceQuery.
/// </summary>
public class GetProfitBySourceQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IInventoryItemRepository _inventoryRepository;

    public GetProfitBySourceQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IInventoryItemRepository inventoryRepository)
    {
        _revenueRepository = revenueRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<ProfitBySourceDto>> HandleAsync(
        GetProfitBySourceQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = new List<ProfitBySourceDto>();

        // Get all revenue entries for the date range
        var revenueEntries = await _revenueRepository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        // Filter by currency
        var filteredEntries = revenueEntries
            .Where(r => r.GrossAmount.Currency == query.Currency)
            .ToList();

        // Calculate eBay profit
        var ebayEntries = filteredEntries.Where(r => r.Source == RevenueSource.eBay).ToList();
        if (ebayEntries.Count > 0)
        {
            var grossRevenue = ebayEntries.Sum(r => r.GrossAmount.Amount);
            var fees = ebayEntries.Sum(r => r.Fees.Amount);
            var netRevenue = grossRevenue - fees;

            // Get COGS for linked inventory items
            var cogs = 0m;
            foreach (var entry in ebayEntries.Where(e => e.InventoryItemId.HasValue))
            {
                var item = await _inventoryRepository.GetByIdAsync(entry.InventoryItemId!.Value, cancellationToken);
                if (item != null && item.Cogs.Currency == query.Currency)
                {
                    cogs += item.Cogs.Amount;
                }
            }

            var profit = netRevenue - cogs;
            var profitMargin = grossRevenue > 0 ? (profit / grossRevenue) * 100 : 0;

            result.Add(new ProfitBySourceDto
            {
                Source = RevenueSource.eBay,
                SourceDisplay = "eBay",
                GrossRevenue = grossRevenue,
                Fees = fees,
                NetRevenue = netRevenue,
                Cogs = cogs,
                Expenses = 0, // eBay expenses tracked separately in BusinessExpenses
                Profit = profit,
                ProfitMargin = profitMargin,
                Currency = query.Currency,
                GrossRevenueFormatted = new Money(grossRevenue, query.Currency).ToString(),
                FeesFormatted = new Money(fees, query.Currency).ToString(),
                NetRevenueFormatted = new Money(netRevenue, query.Currency).ToString(),
                CogsFormatted = new Money(cogs, query.Currency).ToString(),
                ExpensesFormatted = new Money(0, query.Currency).ToString(),
                ProfitFormatted = new Money(profit, query.Currency).ToString(),
                TransactionCount = ebayEntries.Count
            });
        }

        // Calculate Service profit
        var serviceEntries = filteredEntries.Where(r => r.Source == RevenueSource.ComputerServices).ToList();
        if (serviceEntries.Count > 0)
        {
            var grossRevenue = serviceEntries.Sum(r => r.GrossAmount.Amount);
            var fees = serviceEntries.Sum(r => r.Fees.Amount);
            var netRevenue = grossRevenue - fees;

            // Service profit calculation - expenses tracked separately
            var profit = netRevenue;
            var profitMargin = grossRevenue > 0 ? (profit / grossRevenue) * 100 : 0;

            result.Add(new ProfitBySourceDto
            {
                Source = RevenueSource.ComputerServices,
                SourceDisplay = "Computer Services",
                GrossRevenue = grossRevenue,
                Fees = fees,
                NetRevenue = netRevenue,
                Cogs = 0, // Services don't have traditional COGS
                Expenses = 0, // Will be populated when expenses are linked
                Profit = profit,
                ProfitMargin = profitMargin,
                Currency = query.Currency,
                GrossRevenueFormatted = new Money(grossRevenue, query.Currency).ToString(),
                FeesFormatted = new Money(fees, query.Currency).ToString(),
                NetRevenueFormatted = new Money(netRevenue, query.Currency).ToString(),
                CogsFormatted = new Money(0, query.Currency).ToString(),
                ExpensesFormatted = new Money(0, query.Currency).ToString(),
                ProfitFormatted = new Money(profit, query.Currency).ToString(),
                TransactionCount = serviceEntries.Count
            });
        }

        return result;
    }
}
