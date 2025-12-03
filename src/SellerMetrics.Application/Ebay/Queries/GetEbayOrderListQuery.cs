using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Ebay.Queries;

/// <summary>
/// Query to get a list of eBay orders for a user.
/// </summary>
public record GetEbayOrderListQuery(
    string UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    EbayOrderStatus? Status = null,
    bool? LinkedOnly = null);

/// <summary>
/// Handler for GetEbayOrderListQuery.
/// </summary>
public class GetEbayOrderListQueryHandler
{
    private readonly IEbayOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryRepository;

    public GetEbayOrderListQueryHandler(
        IEbayOrderRepository orderRepository,
        IInventoryItemRepository inventoryRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<EbayOrderSummaryDto>> HandleAsync(
        GetEbayOrderListQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<EbayOrder> orders;

        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            orders = await _orderRepository.GetByDateRangeAsync(
                query.UserId,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else if (query.Status.HasValue)
        {
            orders = await _orderRepository.GetByStatusAsync(
                query.UserId,
                query.Status.Value,
                cancellationToken);
        }
        else
        {
            orders = await _orderRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        }

        // Filter by linked status if specified
        if (query.LinkedOnly == true)
        {
            orders = orders.Where(o => o.InventoryItemId.HasValue).ToList();
        }
        else if (query.LinkedOnly == false)
        {
            orders = orders.Where(o => !o.InventoryItemId.HasValue).ToList();
        }

        return orders.Select(MapToSummaryDto).ToList();
    }

    private static EbayOrderSummaryDto MapToSummaryDto(EbayOrder order)
    {
        return new EbayOrderSummaryDto
        {
            Id = order.Id,
            EbayOrderId = order.EbayOrderId,
            OrderDate = order.OrderDate,
            BuyerUsername = order.BuyerUsername,
            ItemTitle = order.ItemTitle,
            GrossSaleFormatted = order.GrossSale.ToString(),
            NetPayoutFormatted = order.NetPayout.ToString(),
            ProfitFormatted = order.Profit?.ToString(),
            IsLinkedToInventory = order.InventoryItemId.HasValue,
            Status = order.Status,
            StatusDisplay = order.Status.ToString()
        };
    }
}
