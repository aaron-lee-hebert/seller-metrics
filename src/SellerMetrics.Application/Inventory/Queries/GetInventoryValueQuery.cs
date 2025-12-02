using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Inventory.Queries;

/// <summary>
/// Query to get the total value of unsold inventory.
/// </summary>
public record GetInventoryValueQuery(string Currency = "USD");

/// <summary>
/// Handler for GetInventoryValueQuery.
/// </summary>
public class GetInventoryValueQueryHandler
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryValueQueryHandler(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryValueDto> HandleAsync(
        GetInventoryValueQuery query,
        CancellationToken cancellationToken = default)
    {
        var totalValue = await _repository.GetTotalInventoryValueAsync(query.Currency, cancellationToken);

        // Get item counts
        var allItems = await _repository.GetAllAsync(cancellationToken);
        var unsoldItems = allItems.Where(i => i.Status != InventoryStatus.Sold).ToList();

        return new InventoryValueDto
        {
            TotalValue = totalValue,
            Currency = query.Currency,
            TotalValueFormatted = new Money(totalValue, query.Currency).ToString(),
            TotalItems = unsoldItems.Count,
            TotalQuantity = unsoldItems.Sum(i => i.Quantity),
            UnlistedCount = unsoldItems.Count(i => i.Status == InventoryStatus.Unlisted),
            ListedCount = unsoldItems.Count(i => i.Status == InventoryStatus.Listed)
        };
    }
}
