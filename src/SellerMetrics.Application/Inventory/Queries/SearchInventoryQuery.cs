using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Queries;

/// <summary>
/// Query to search inventory items by title, SKU, or notes.
/// </summary>
public record SearchInventoryQuery(string SearchTerm);

/// <summary>
/// Handler for SearchInventoryQuery.
/// </summary>
public class SearchInventoryQueryHandler
{
    private readonly IInventoryItemRepository _repository;

    public SearchInventoryQueryHandler(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> HandleAsync(
        SearchInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            return Array.Empty<InventoryItemDto>();
        }

        var items = await _repository.SearchAsync(query.SearchTerm, cancellationToken);
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
