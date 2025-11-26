using SellerMetrics.Application.Profit.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Profit.Queries;

/// <summary>
/// Query to get profit summary for tax reporting.
/// Aggregates data in Schedule C format.
/// </summary>
public record GetTaxReportProfitQuery(
    int FiscalYear,
    string Currency = "USD");

/// <summary>
/// Handler for GetTaxReportProfitQuery.
/// </summary>
public class GetTaxReportProfitQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IFiscalYearConfigurationRepository _fiscalYearRepository;

    public GetTaxReportProfitQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IInventoryItemRepository inventoryRepository,
        IFiscalYearConfigurationRepository fiscalYearRepository)
    {
        _revenueRepository = revenueRepository;
        _inventoryRepository = inventoryRepository;
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<TaxReportProfitDto> HandleAsync(
        GetTaxReportProfitQuery query,
        CancellationToken cancellationToken = default)
    {
        var fiscalConfig = await _fiscalYearRepository.GetActiveAsync(cancellationToken)
            ?? new FiscalYearConfiguration { FiscalYearStartMonth = 1 };

        var startDate = fiscalConfig.GetFiscalYearStart(query.FiscalYear);
        var endDate = fiscalConfig.GetFiscalYearEnd(query.FiscalYear);

        var entries = await _revenueRepository.GetByDateRangeAsync(
            startDate,
            endDate,
            cancellationToken);

        var filteredEntries = entries
            .Where(r => r.GrossAmount.Currency == query.Currency)
            .ToList();

        // Calculate gross receipts (all gross revenue)
        var totalGrossReceipts = filteredEntries.Sum(r => r.GrossAmount.Amount);

        // Returns and allowances (fees in this context)
        var totalFees = filteredEntries.Sum(r => r.Fees.Amount);

        // Net gross receipts
        var netGrossReceipts = totalGrossReceipts - totalFees;

        // Cost of goods sold (COGS from eBay inventory sold)
        var cogs = 0m;
        foreach (var entry in filteredEntries.Where(e => e.InventoryItemId.HasValue))
        {
            var item = await _inventoryRepository.GetByIdAsync(entry.InventoryItemId!.Value, cancellationToken);
            if (item != null && item.Cogs.Currency == query.Currency)
            {
                cogs += item.Cogs.Amount;
            }
        }

        // Gross profit
        var grossProfit = netGrossReceipts - cogs;

        // Total expenses (will be populated from BusinessExpenses when available)
        var totalExpenses = 0m;

        // Net profit
        var netProfit = grossProfit - totalExpenses;

        // Build quarterly breakdown
        var quarterlyBreakdown = new List<QuarterlyProfitDto>();
        var quarterlyHandler = new GetQuarterlyProfitQueryHandler(
            _revenueRepository,
            _inventoryRepository,
            _fiscalYearRepository);

        var quarterlyResult = await quarterlyHandler.HandleAsync(
            new GetQuarterlyProfitQuery(query.FiscalYear, query.Currency),
            cancellationToken);

        return new TaxReportProfitDto
        {
            FiscalYear = query.FiscalYear,
            TotalGrossReceipts = totalGrossReceipts,
            TotalReturnsAndAllowances = totalFees,
            NetGrossReceipts = netGrossReceipts,
            CostOfGoodsSold = cogs,
            GrossProfit = grossProfit,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            Currency = query.Currency,
            TotalGrossReceiptsFormatted = new Money(totalGrossReceipts, query.Currency).ToString(),
            CostOfGoodsSoldFormatted = new Money(cogs, query.Currency).ToString(),
            GrossProfitFormatted = new Money(grossProfit, query.Currency).ToString(),
            TotalExpensesFormatted = new Money(totalExpenses, query.Currency).ToString(),
            NetProfitFormatted = new Money(netProfit, query.Currency).ToString(),
            QuarterlyBreakdown = quarterlyResult
        };
    }
}
