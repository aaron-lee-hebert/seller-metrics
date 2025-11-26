using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to move an inventory item to a different storage location.
/// </summary>
public record MoveInventoryItemCommand(int Id, int? NewStorageLocationId);

/// <summary>
/// Handler for MoveInventoryItemCommand.
/// </summary>
public class MoveInventoryItemCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        MoveInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Inventory item with ID {command.Id} not found.");
        }

        // Validate new storage location exists if specified
        if (command.NewStorageLocationId.HasValue)
        {
            var storageLocation = await _storageLocationRepository.GetByIdAsync(
                command.NewStorageLocationId.Value, cancellationToken);
            if (storageLocation == null)
            {
                throw new ArgumentException($"Storage location with ID {command.NewStorageLocationId} not found.");
            }
        }

        item.MoveTo(command.NewStorageLocationId);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
