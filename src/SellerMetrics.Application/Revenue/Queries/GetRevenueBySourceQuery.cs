using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get revenue summary by source for a date range.
/// </summary>
public record GetRevenueBySourceQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetRevenueBySourceQuery.
/// </summary>
public class GetRevenueBySourceQueryHandler
{
    private readonly IRevenueEntryRepository _repository;

    public GetRevenueBySourceQueryHandler(IRevenueEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RevenueBySourceDto>> HandleAsync(
        GetRevenueBySourceQuery query,
        CancellationToken cancellationToken = default)
    {
        var entries = await _repository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        var result = new List<RevenueBySourceDto>();

        // Group by source and calculate totals
        var sources = new[] { RevenueSource.eBay, RevenueSource.ComputerServices };
        foreach (var source in sources)
        {
            var sourceEntries = entries
                .Where(e => e.Source == source && e.GrossAmount.Currency == query.Currency)
                .ToList();

            if (sourceEntries.Count > 0)
            {
                var grossTotal = sourceEntries.Sum(e => e.GrossAmount.Amount);
                var feesTotal = sourceEntries.Sum(e => e.Fees.Amount);
                var netTotal = grossTotal - feesTotal;

                result.Add(new RevenueBySourceDto
                {
                    Source = source,
                    SourceDisplay = source == RevenueSource.eBay ? "eBay" : "Computer Services",
                    GrossTotal = grossTotal,
                    FeesTotal = feesTotal,
                    NetTotal = netTotal,
                    Currency = query.Currency,
                    GrossFormatted = new Money(grossTotal, query.Currency).ToString(),
                    FeesFormatted = new Money(feesTotal, query.Currency).ToString(),
                    NetFormatted = new Money(netTotal, query.Currency).ToString(),
                    TransactionCount = sourceEntries.Count
                });
            }
        }

        return result;
    }
}
