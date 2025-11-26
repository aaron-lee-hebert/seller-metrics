using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Queries;

/// <summary>
/// Query to get a single storage location by ID.
/// </summary>
public record GetStorageLocationQuery(int Id);

/// <summary>
/// Handler for GetStorageLocationQuery.
/// </summary>
public class GetStorageLocationQueryHandler
{
    private readonly IStorageLocationRepository _repository;

    public GetStorageLocationQueryHandler(IStorageLocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<StorageLocationDto?> HandleAsync(
        GetStorageLocationQuery query,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetWithAncestorsAsync(query.Id, cancellationToken);
        return location != null ? MapToDto(location) : null;
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
            UpdatedAt = location.UpdatedAt
        };
    }
}
