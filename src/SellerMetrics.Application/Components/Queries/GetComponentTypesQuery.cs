using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Queries;

/// <summary>
/// Query to get all component types.
/// </summary>
public record GetComponentTypesQuery;

/// <summary>
/// Handler for GetComponentTypesQuery.
/// </summary>
public class GetComponentTypesQueryHandler
{
    private readonly IComponentTypeRepository _repository;

    public GetComponentTypesQueryHandler(IComponentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ComponentTypeDto>> HandleAsync(
        GetComponentTypesQuery query,
        CancellationToken cancellationToken = default)
    {
        var types = await _repository.GetAllOrderedAsync(cancellationToken);

        return types.Select(t => new ComponentTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            DefaultExpenseCategory = t.DefaultExpenseCategory,
            IsSystemType = t.IsSystemType,
            SortOrder = t.SortOrder
        }).ToList();
    }
}
