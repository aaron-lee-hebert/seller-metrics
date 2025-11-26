using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Commands;

/// <summary>
/// Command to update an existing storage location.
/// </summary>
public record UpdateStorageLocationCommand(
    int Id,
    string Name,
    string? Description,
    int? ParentId,
    int SortOrder);

/// <summary>
/// Handler for UpdateStorageLocationCommand.
/// </summary>
public class UpdateStorageLocationCommandHandler
{
    private readonly IStorageLocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStorageLocationCommandHandler(
        IStorageLocationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StorageLocationDto> HandleAsync(
        UpdateStorageLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetWithAncestorsAsync(command.Id, cancellationToken);
        if (location == null)
        {
            throw new ArgumentException($"Storage location with ID {command.Id} not found.");
        }

        // Validate parent exists if specified and is not creating a circular reference
        if (command.ParentId.HasValue)
        {
            if (command.ParentId == command.Id)
            {
                throw new ArgumentException("A storage location cannot be its own parent.");
            }

            var parent = await _repository.GetWithAncestorsAsync(command.ParentId.Value, cancellationToken);
            if (parent == null)
            {
                throw new ArgumentException($"Parent storage location with ID {command.ParentId} not found.");
            }

            // Check for circular reference (parent cannot be a descendant of this location)
            if (await IsDescendantOfAsync(parent, command.Id, cancellationToken))
            {
                throw new ArgumentException("Cannot set parent: would create a circular reference in the hierarchy.");
            }

            location.Parent = parent;
        }
        else
        {
            location.Parent = null;
        }

        location.Name = command.Name;
        location.Description = command.Description;
        location.ParentId = command.ParentId;
        location.SortOrder = command.SortOrder;
        location.UpdatedAt = DateTime.UtcNow;

        _repository.Update(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(location);
    }

    private async Task<bool> IsDescendantOfAsync(
        Domain.Entities.StorageLocation location,
        int potentialAncestorId,
        CancellationToken cancellationToken)
    {
        var current = location;
        while (current.ParentId.HasValue)
        {
            if (current.ParentId == potentialAncestorId)
            {
                return true;
            }

            current = await _repository.GetByIdAsync(current.ParentId.Value, cancellationToken);
            if (current == null) break;
        }
        return false;
    }

    private static StorageLocationDto MapToDto(Domain.Entities.StorageLocation location)
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
