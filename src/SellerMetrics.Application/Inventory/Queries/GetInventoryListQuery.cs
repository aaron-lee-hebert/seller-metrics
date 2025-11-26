using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Queries;

/// <summary>
/// Query to get inventory items with optional filters.
/// </summary>
public record GetInventoryListQuery(
    InventoryStatus? Status = null,
    int? StorageLocationId = null);

/// <summary>
/// Handler for GetInventoryListQuery.
/// </summary>
public class GetInventoryListQueryHandler
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryListQueryHandler(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> HandleAsync(
        GetInventoryListQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<InventoryItem> items;

        if (query.Status.HasValue)
        {
            items = await _repository.GetByStatusAsync(query.Status.Value, cancellationToken);
        }
        else if (query.StorageLocationId.HasValue)
        {
            items = await _repository.GetByLocationAsync(query.StorageLocationId.Value, cancellationToken);
        }
        else
        {
            items = await _repository.GetAllAsync(cancellationToken);
        }

        return items.Select(MapToDto).ToList();
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
