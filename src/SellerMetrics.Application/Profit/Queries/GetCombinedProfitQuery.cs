using SellerMetrics.Application.Profit.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Profit.Queries;

/// <summary>
/// Query to get combined profit across all sources for a date range.
/// </summary>
public record GetCombinedProfitQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetCombinedProfitQuery.
/// </summary>
public class GetCombinedProfitQueryHandler
{
    private readonly GetProfitBySourceQueryHandler _bySourceHandler;

    public GetCombinedProfitQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IInventoryItemRepository inventoryRepository)
    {
        _bySourceHandler = new GetProfitBySourceQueryHandler(revenueRepository, inventoryRepository);
    }

    public async Task<CombinedProfitDto> HandleAsync(
        GetCombinedProfitQuery query,
        CancellationToken cancellationToken = default)
    {
        var bySourceQuery = new GetProfitBySourceQuery(query.StartDate, query.EndDate, query.Currency);
        var sourceResults = await _bySourceHandler.HandleAsync(bySourceQuery, cancellationToken);

        var ebayProfit = sourceResults.FirstOrDefault(s => s.Source == RevenueSource.eBay);
        var serviceProfit = sourceResults.FirstOrDefault(s => s.Source == RevenueSource.ComputerServices);

        var totalGrossRevenue = sourceResults.Sum(s => s.GrossRevenue);
        var totalFees = sourceResults.Sum(s => s.Fees);
        var totalNetRevenue = sourceResults.Sum(s => s.NetRevenue);
        var totalCogs = sourceResults.Sum(s => s.Cogs);
        var totalExpenses = sourceResults.Sum(s => s.Expenses);
        var totalProfit = sourceResults.Sum(s => s.Profit);
        var overallMargin = totalGrossRevenue > 0 ? (totalProfit / totalGrossRevenue) * 100 : 0;

        return new CombinedProfitDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TotalGrossRevenue = totalGrossRevenue,
            TotalFees = totalFees,
            TotalNetRevenue = totalNetRevenue,
            TotalCogs = totalCogs,
            TotalExpenses = totalExpenses,
            TotalProfit = totalProfit,
            OverallProfitMargin = overallMargin,
            Currency = query.Currency,
            TotalGrossRevenueFormatted = new Money(totalGrossRevenue, query.Currency).ToString(),
            TotalFeesFormatted = new Money(totalFees, query.Currency).ToString(),
            TotalNetRevenueFormatted = new Money(totalNetRevenue, query.Currency).ToString(),
            TotalCogsFormatted = new Money(totalCogs, query.Currency).ToString(),
            TotalExpensesFormatted = new Money(totalExpenses, query.Currency).ToString(),
            TotalProfitFormatted = new Money(totalProfit, query.Currency).ToString(),
            EbayProfit = ebayProfit,
            ServiceProfit = serviceProfit
        };
    }
}
