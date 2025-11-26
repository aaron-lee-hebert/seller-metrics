using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.StorageLocations.Commands;

/// <summary>
/// Command to soft-delete a storage location.
/// Will fail if the location has items or children.
/// </summary>
public record DeleteStorageLocationCommand(int Id);

/// <summary>
/// Handler for DeleteStorageLocationCommand.
/// </summary>
public class DeleteStorageLocationCommandHandler
{
    private readonly IStorageLocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStorageLocationCommandHandler(
        IStorageLocationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteStorageLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (location == null)
        {
            throw new ArgumentException($"Storage location with ID {command.Id} not found.");
        }

        // Check for children
        if (await _repository.HasChildrenAsync(command.Id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Cannot delete storage location: it has child locations. " +
                "Please delete or move all child locations first.");
        }

        // Check for items
        if (await _repository.HasItemsAsync(command.Id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Cannot delete storage location: it contains inventory items or components. " +
                "Please move all items to another location first.");
        }

        // Soft delete
        location.IsDeleted = true;
        location.DeletedAt = DateTime.UtcNow;
        location.UpdatedAt = DateTime.UtcNow;

        _repository.Update(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
