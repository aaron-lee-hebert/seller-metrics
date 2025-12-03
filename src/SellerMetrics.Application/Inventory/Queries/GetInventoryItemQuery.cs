using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Queries;

/// <summary>
/// Query to get a single inventory item by ID.
/// </summary>
public record GetInventoryItemQuery(int Id);

/// <summary>
/// Handler for GetInventoryItemQuery.
/// </summary>
public class GetInventoryItemQueryHandler
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryItemQueryHandler(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryItemDto?> HandleAsync(
        GetInventoryItemQuery query,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return item != null ? MapToDto(item) : null;
    }

    private static InventoryItemDto MapToDto(InventoryItem item)
    {
        return new InventoryItemDto
        {
            Id = item.Id,
            InternalSku = item.InternalSku,
            EbaySku = item.EbaySku,
            EffectiveSku = item.EffectiveSku,
            Title = item.Title,
            Description = item.Description,
            CogsAmount = item.Cogs.Amount,
            CogsCurrency = item.Cogs.Currency,
            CogsFormatted = item.Cogs.ToString(),
            Quantity = item.Quantity,
            TotalValueAmount = item.TotalValue.Amount,
            TotalValueFormatted = item.TotalValue.ToString(),
            PurchaseDate = item.PurchaseDate,
            StorageLocationId = item.StorageLocationId,
            StorageLocationPath = item.StorageLocation?.FullPath,
            Status = item.Status,
            StatusDisplay = item.Status.ToString(),
            Condition = item.Condition,
            ConditionDisplay = item.Condition?.GetDisplayName(),
            Notes = item.Notes,
            PhotoPath = item.PhotoPath,
            EbayListingId = item.EbayListingId,
            ListedDate = item.ListedDate,
            SoldDate = item.SoldDate,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
