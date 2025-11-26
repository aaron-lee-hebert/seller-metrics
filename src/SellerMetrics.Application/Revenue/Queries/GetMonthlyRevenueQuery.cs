using System.Globalization;
using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get monthly revenue breakdown for a date range.
/// </summary>
public record GetMonthlyRevenueQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetMonthlyRevenueQuery.
/// </summary>
public class GetMonthlyRevenueQueryHandler
{
    private readonly IRevenueEntryRepository _repository;

    public GetMonthlyRevenueQueryHandler(IRevenueEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MonthlyRevenueDto>> HandleAsync(
        GetMonthlyRevenueQuery query,
        CancellationToken cancellationToken = default)
    {
        var entries = await _repository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        // Filter by currency
        var filteredEntries = entries
            .Where(e => e.GrossAmount.Currency == query.Currency)
            .ToList();

        // Group by year/month
        var monthlyData = filteredEntries
            .GroupBy(e => new { e.TransactionDate.Year, e.TransactionDate.Month })
            .Select(g =>
            {
                var ebayRevenue = g
                    .Where(e => e.Source == RevenueSource.eBay)
                    .Sum(e => e.GrossAmount.Amount - e.Fees.Amount);

                var serviceRevenue = g
                    .Where(e => e.Source == RevenueSource.ComputerServices)
                    .Sum(e => e.GrossAmount.Amount - e.Fees.Amount);

                return new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    EbayRevenue = ebayRevenue,
                    ServiceRevenue = serviceRevenue,
                    TotalRevenue = ebayRevenue + serviceRevenue,
                    Currency = query.Currency,
                    EbayRevenueFormatted = new Money(ebayRevenue, query.Currency).ToString(),
                    ServiceRevenueFormatted = new Money(serviceRevenue, query.Currency).ToString(),
                    TotalRevenueFormatted = new Money(ebayRevenue + serviceRevenue, query.Currency).ToString()
                };
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        return monthlyData;
    }
}
