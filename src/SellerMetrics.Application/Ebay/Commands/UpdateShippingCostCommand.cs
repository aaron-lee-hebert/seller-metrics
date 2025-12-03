using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Ebay.Commands;

/// <summary>
/// Command to update the actual shipping cost for an eBay order.
/// </summary>
public record UpdateShippingCostCommand(
    int OrderId,
    decimal ShippingAmount,
    string Currency,
    string UserId);

/// <summary>
/// Handler for UpdateShippingCostCommand.
/// </summary>
public class UpdateShippingCostCommandHandler
{
    private readonly IEbayOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShippingCostCommandHandler(
        IEbayOrderRepository orderRepository,
        IInventoryItemRepository inventoryRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EbayOrderDisplayDto> HandleAsync(
        UpdateShippingCostCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order == null)
        {
            throw new ArgumentException($"Order with ID {command.OrderId} not found.");
        }

        if (order.UserId != command.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this order.");
        }

        order.UpdateShippingCost(new Money(command.ShippingAmount, command.Currency));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get linked inventory for DTO
        Domain.Entities.InventoryItem? inventory = null;
        if (order.InventoryItemId.HasValue)
        {
            inventory = await _inventoryRepository.GetByIdAsync(order.InventoryItemId.Value, cancellationToken);
        }

        return MapToDto(order, inventory);
    }

    private static EbayOrderDisplayDto MapToDto(Domain.Entities.EbayOrder order, Domain.Entities.InventoryItem? inventory)
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
