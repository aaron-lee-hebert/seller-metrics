using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Components.Queries;

/// <summary>
/// Query to get the total value of available components.
/// </summary>
public record GetComponentValueQuery(string Currency = "USD");

/// <summary>
/// Handler for GetComponentValueQuery.
/// </summary>
public class GetComponentValueQueryHandler
{
    private readonly IComponentItemRepository _repository;

    public GetComponentValueQueryHandler(IComponentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<ComponentValueDto> HandleAsync(
        GetComponentValueQuery query,
        CancellationToken cancellationToken = default)
    {
        var totalValue = await _repository.GetTotalComponentValueAsync(query.Currency, cancellationToken);

        // Get item counts
        var allItems = await _repository.GetAllAsync(cancellationToken);
        var availableItems = allItems.Where(i => i.Status == ComponentStatus.Available).ToList();
        var lowStockItems = await _repository.GetLowStockAsync(1, cancellationToken);

        return new ComponentValueDto
        {
            TotalValue = totalValue,
            Currency = query.Currency,
            TotalValueFormatted = new Money(totalValue, query.Currency).ToString(),
            TotalItems = availableItems.Count,
            TotalQuantity = availableItems.Sum(i => i.Quantity),
            LowStockCount = lowStockItems.Count
        };
    }
}
