using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to soft-delete a component item.
/// </summary>
public record DeleteComponentItemCommand(int Id);

/// <summary>
/// Handler for DeleteComponentItemCommand.
/// </summary>
public class DeleteComponentItemCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteComponentItemCommandHandler(
        IComponentItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteComponentItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var component = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (component == null)
        {
            throw new ArgumentException($"Component item with ID {command.Id} not found.");
        }

        // Soft delete
        component.IsDeleted = true;
        component.DeletedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;

        _repository.Update(component);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
