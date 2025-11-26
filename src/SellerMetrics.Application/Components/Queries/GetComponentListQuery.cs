using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Queries;

/// <summary>
/// Query to get component items with optional filters.
/// </summary>
public record GetComponentListQuery(
    int? ComponentTypeId = null,
    int? StorageLocationId = null,
    ComponentStatus? Status = null);

/// <summary>
/// Handler for GetComponentListQuery.
/// </summary>
public class GetComponentListQueryHandler
{
    private readonly IComponentItemRepository _repository;

    public GetComponentListQueryHandler(IComponentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ComponentItemDto>> HandleAsync(
        GetComponentListQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ComponentItem> items;

        if (query.ComponentTypeId.HasValue)
        {
            items = await _repository.GetByTypeAsync(query.ComponentTypeId.Value, cancellationToken);
        }
        else if (query.StorageLocationId.HasValue)
        {
            items = await _repository.GetByLocationAsync(query.StorageLocationId.Value, cancellationToken);
        }
        else if (query.Status.HasValue)
        {
            items = await _repository.GetByStatusAsync(query.Status.Value, cancellationToken);
        }
        else
        {
            items = await _repository.GetAllAsync(cancellationToken);
        }

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
            ServiceJobId = item.ServiceJobId,
            ServiceJobNumber = item.ServiceJob?.JobNumber,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
