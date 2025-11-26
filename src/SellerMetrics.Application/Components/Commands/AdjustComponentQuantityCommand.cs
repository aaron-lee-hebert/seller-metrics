using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to adjust the quantity of a component item.
/// Creates an audit record for tracking.
/// </summary>
public record AdjustComponentQuantityCommand(
    int Id,
    int Adjustment,
    string Reason);

/// <summary>
/// Handler for AdjustComponentQuantityCommand.
/// </summary>
public class AdjustComponentQuantityCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustComponentQuantityCommandHandler(
        IComponentItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        AdjustComponentQuantityCommand command,
        CancellationToken cancellationToken = default)
    {
        var component = await _repository.GetWithDetailsAsync(command.Id, cancellationToken);
        if (component == null)
        {
            throw new ArgumentException($"Component item with ID {command.Id} not found.");
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("A reason must be provided for quantity adjustments.");
        }

        // This creates an audit record and updates the quantity
        var adjustment = component.AdjustQuantity(command.Adjustment, command.Reason);
        component.Adjustments.Add(adjustment);

        _repository.Update(component);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
