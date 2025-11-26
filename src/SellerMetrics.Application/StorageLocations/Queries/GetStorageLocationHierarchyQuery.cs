using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Queries;

/// <summary>
/// Query to get the full storage location hierarchy as a tree structure.
/// </summary>
public record GetStorageLocationHierarchyQuery;

/// <summary>
/// Handler for GetStorageLocationHierarchyQuery.
/// </summary>
public class GetStorageLocationHierarchyQueryHandler
{
    private readonly IStorageLocationRepository _repository;

    public GetStorageLocationHierarchyQueryHandler(IStorageLocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<StorageLocationDto>> HandleAsync(
        GetStorageLocationHierarchyQuery query,
        CancellationToken cancellationToken = default)
    {
        var hierarchy = await _repository.GetHierarchyAsync(cancellationToken);
        return hierarchy.Select(MapToDto).ToList();
    }

    private static StorageLocationDto MapToDto(StorageLocation location)
    {
        return new StorageLocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            ParentId = location.ParentId,
            ParentName = location.Parent?.Name,
            FullPath = location.FullPath,
            Depth = location.Depth,
            SortOrder = location.SortOrder,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt,
            Children = location.Children
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(MapToDto)
                .ToList()
        };
    }
}
