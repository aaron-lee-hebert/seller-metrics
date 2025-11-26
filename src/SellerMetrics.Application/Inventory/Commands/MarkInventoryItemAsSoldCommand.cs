using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to mark an inventory item as sold.
/// </summary>
public record MarkInventoryItemAsSoldCommand(int Id);

/// <summary>
/// Handler for MarkInventoryItemAsSoldCommand.
/// </summary>
public class MarkInventoryItemAsSoldCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkInventoryItemAsSoldCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        MarkInventoryItemAsSoldCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Inventory item with ID {command.Id} not found.");
        }

        if (item.Status == InventoryStatus.Sold)
        {
            throw new InvalidOperationException($"Inventory item with ID {command.Id} is already marked as sold.");
        }

        item.MarkAsSold();

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
