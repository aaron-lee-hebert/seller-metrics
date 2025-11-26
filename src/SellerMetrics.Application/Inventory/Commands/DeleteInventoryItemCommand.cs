using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to soft-delete an inventory item.
/// </summary>
public record DeleteInventoryItemCommand(int Id);

/// <summary>
/// Handler for DeleteInventoryItemCommand.
/// </summary>
public class DeleteInventoryItemCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Inventory item with ID {command.Id} not found.");
        }

        // Soft delete
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
