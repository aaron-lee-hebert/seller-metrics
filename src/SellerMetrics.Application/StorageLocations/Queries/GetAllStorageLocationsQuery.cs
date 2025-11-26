using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Queries;

/// <summary>
/// Query to get all storage locations as a flat list (useful for dropdowns).
/// </summary>
public record GetAllStorageLocationsQuery;

/// <summary>
/// Handler for GetAllStorageLocationsQuery.
/// </summary>
public class GetAllStorageLocationsQueryHandler
{
    private readonly IStorageLocationRepository _repository;

    public GetAllStorageLocationsQueryHandler(IStorageLocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<StorageLocationSummaryDto>> HandleAsync(
        GetAllStorageLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get hierarchy to compute full paths
        var hierarchy = await _repository.GetHierarchyAsync(cancellationToken);
        var flatList = new List<StorageLocationSummaryDto>();

        // Flatten the hierarchy while preserving order
        FlattenHierarchy(hierarchy, flatList);

        return flatList;
    }

    private static void FlattenHierarchy(
        IEnumerable<StorageLocation> locations,
        List<StorageLocationSummaryDto> flatList)
    {
        foreach (var location in locations.OrderBy(l => l.SortOrder).ThenBy(l => l.Name))
        {
            flatList.Add(new StorageLocationSummaryDto
            {
                Id = location.Id,
                Name = location.Name,
                FullPath = location.FullPath,
                Depth = location.Depth
            });

            if (location.Children.Any())
            {
                FlattenHierarchy(location.Children, flatList);
            }
        }
    }
}
