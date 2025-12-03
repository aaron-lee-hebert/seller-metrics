using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Ebay.Queries;

/// <summary>
/// Query to get a single eBay order by ID.
/// </summary>
public record GetEbayOrderQuery(int OrderId, string UserId);

/// <summary>
/// Handler for GetEbayOrderQuery.
/// </summary>
public class GetEbayOrderQueryHandler
{
    private readonly IEbayOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryRepository;

    public GetEbayOrderQueryHandler(
        IEbayOrderRepository orderRepository,
        IInventoryItemRepository inventoryRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<EbayOrderDisplayDto?> HandleAsync(
        GetEbayOrderQuery query,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(query.OrderId, cancellationToken);
        if (order == null)
        {
            return null;
        }

        if (order.UserId != query.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this order.");
        }

        // Get linked inventory if exists
        InventoryItem? inventory = null;
        if (order.InventoryItemId.HasValue)
        {
            inventory = await _inventoryRepository.GetByIdAsync(order.InventoryItemId.Value, cancellationToken);
        }

        return MapToDto(order, inventory);
    }

    private static EbayOrderDisplayDto MapToDto(EbayOrder order, InventoryItem? inventory)
    {
        return new EbayOrderDisplayDto
        {
            Id = order.Id,
            EbayOrderId = order.EbayOrderId,
            LegacyOrderId = order.LegacyOrderId,
            OrderDate = order.OrderDate,
            BuyerUsername = order.BuyerUsername,
            ItemTitle = order.ItemTitle,
            EbayItemId = order.EbayItemId,
            Sku = order.Sku,
            Quantity = order.Quantity,
            GrossSaleAmount = order.GrossSale.Amount,
            GrossSaleFormatted = order.GrossSale.ToString(),
            ShippingPaidAmount = order.ShippingPaid.Amount,
            ShippingPaidFormatted = order.ShippingPaid.ToString(),
            ShippingActualAmount = order.ShippingActual.Amount,
            ShippingActualFormatted = order.ShippingActual.ToString(),
            TotalFeesAmount = order.TotalFees.Amount,
            TotalFeesFormatted = order.TotalFees.ToString(),
            NetPayoutAmount = order.NetPayout.Amount,
            NetPayoutFormatted = order.NetPayout.ToString(),
            CogsAmount = inventory?.Cogs.Amount,
            CogsFormatted = inventory?.Cogs.ToString(),
            ProfitAmount = order.Profit?.Amount,
            ProfitFormatted = order.Profit?.ToString(),
            ProfitMargin = order.ProfitMargin,
            ProfitMarginFormatted = order.ProfitMargin.HasValue ? $"{order.ProfitMargin:F1}%" : null,
            InventoryItemId = order.InventoryItemId,
            InventoryItemTitle = inventory?.Title,
            InventoryItemSku = inventory?.EffectiveSku,
            Status = order.Status,
            StatusDisplay = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus,
            PaymentStatusDisplay = order.PaymentStatus.ToString(),
            FulfillmentStatus = order.FulfillmentStatus,
            FulfillmentStatusDisplay = order.FulfillmentStatus.ToString(),
            Notes = order.Notes,
            LastSyncedAt = order.LastSyncedAt,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Currency = order.GrossSale.Currency
        };
    }
}
