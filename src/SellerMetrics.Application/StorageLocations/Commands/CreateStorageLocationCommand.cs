using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Commands;

/// <summary>
/// Command to create a new storage location.
/// </summary>
public record CreateStorageLocationCommand(
    string Name,
    string? Description,
    int? ParentId,
    int SortOrder = 0);

/// <summary>
/// Handler for CreateStorageLocationCommand.
/// </summary>
public class CreateStorageLocationCommandHandler
{
    private readonly IStorageLocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStorageLocationCommandHandler(
        IStorageLocationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StorageLocationDto> HandleAsync(
        CreateStorageLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate parent exists if specified
        StorageLocation? parent = null;
        if (command.ParentId.HasValue)
        {
            parent = await _repository.GetByIdAsync(command.ParentId.Value, cancellationToken);
            if (parent == null)
            {
                throw new ArgumentException($"Parent storage location with ID {command.ParentId} not found.");
            }
        }

        var location = new StorageLocation
        {
            Name = command.Name,
            Description = command.Description,
            ParentId = command.ParentId,
            SortOrder = command.SortOrder
        };

        await _repository.AddAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load parent for FullPath calculation
        if (parent != null)
        {
            location.Parent = parent;
        }

        return MapToDto(location);
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
