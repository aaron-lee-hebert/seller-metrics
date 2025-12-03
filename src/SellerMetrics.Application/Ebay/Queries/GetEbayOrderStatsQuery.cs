using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Ebay.Queries;

/// <summary>
/// Query to get statistics for eBay orders.
/// </summary>
public record GetEbayOrderStatsQuery(
    string UserId,
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetEbayOrderStatsQuery.
/// </summary>
public class GetEbayOrderStatsQueryHandler
{
    private readonly IEbayOrderRepository _orderRepository;

    public GetEbayOrderStatsQueryHandler(IEbayOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<EbayOrderStatsDto> HandleAsync(
        GetEbayOrderStatsQuery query,
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByDateRangeAsync(
            query.UserId,
            query.StartDate,
            query.EndDate,
            cancellationToken);

        var totalOrders = orders.Count;
        var linkedOrders = orders.Count(o => o.InventoryItemId.HasValue);
        var unlinkedOrders = totalOrders - linkedOrders;

        var totalGrossSales = orders
            .Where(o => o.GrossSale.Currency == query.Currency)
            .Sum(o => o.GrossSale.Amount);

        var totalFees = orders
            .Where(o => o.TotalFees.Currency == query.Currency)
            .Sum(o => o.TotalFees.Amount);

        var totalNetPayout = orders
            .Where(o => o.NetPayout.Currency == query.Currency)
            .Sum(o => o.NetPayout.Amount);

        var totalProfit = orders
            .Where(o => o.Profit != null && o.Profit.Currency == query.Currency)
            .Sum(o => o.Profit!.Amount);

        return new EbayOrderStatsDto
        {
            TotalOrders = totalOrders,
            LinkedOrders = linkedOrders,
            UnlinkedOrders = unlinkedOrders,
            TotalGrossSales = totalGrossSales,
            TotalGrossSalesFormatted = new Money(totalGrossSales, query.Currency).ToString(),
            TotalFees = totalFees,
            TotalFeesFormatted = new Money(totalFees, query.Currency).ToString(),
            TotalNetPayout = totalNetPayout,
            TotalNetPayoutFormatted = new Money(totalNetPayout, query.Currency).ToString(),
            TotalProfit = totalProfit,
            TotalProfitFormatted = new Money(totalProfit, query.Currency).ToString(),
            Currency = query.Currency
        };
    }
}
