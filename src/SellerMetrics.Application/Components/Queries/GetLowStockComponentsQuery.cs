using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Queries;

/// <summary>
/// Query to get components with low stock.
/// </summary>
public record GetLowStockComponentsQuery(int Threshold = 1);

/// <summary>
/// Handler for GetLowStockComponentsQuery.
/// </summary>
public class GetLowStockComponentsQueryHandler
{
    private readonly IComponentItemRepository _repository;

    public GetLowStockComponentsQueryHandler(IComponentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ComponentItemDto>> HandleAsync(
        GetLowStockComponentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetLowStockAsync(query.Threshold, cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    private static ComponentItemDto MapToDto(ComponentItem item)
    {
        return new ComponentItemDto
        {
            Id = item.Id,
            ComponentTypeId = item.ComponentTypeId,
            ComponentTypeName = item.ComponentType?.Name ?? string.Empty,
            Description = item.Description,
            Quantity = item.Quantity,
            UnitCostAmount = item.UnitCost.Amount,
            UnitCostCurrency = item.UnitCost.Currency,
            UnitCostFormatted = item.UnitCost.ToString(),
            TotalValueAmount = item.TotalValue.Amount,
            TotalValueFormatted = item.TotalValue.ToString(),
            StorageLocationId = item.StorageLocationId,
            StorageLocationPath = item.StorageLocation?.FullPath,
            Status = item.Status,
            StatusDisplay = item.Status.ToString(),
            AcquiredDate = item.AcquiredDate,
            Source = item.Source,
            SourceDisplay = item.Source.ToString(),
            Notes = item.Notes,
            LowStockThreshold = item.LowStockThreshold,
            IsLowStock = item.IsLowStock,
            ServiceJobId = item.ServiceJobId,
            ServiceJobNumber = item.ServiceJob?.JobNumber,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
