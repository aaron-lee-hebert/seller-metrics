using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to mark a reserved component as used in a service job.
/// </summary>
public record UseComponentCommand(int Id);

/// <summary>
/// Handler for UseComponentCommand.
/// </summary>
public class UseComponentCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UseComponentCommandHandler(
        IComponentItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UseComponentCommand command,
        CancellationToken cancellationToken = default)
    {
        var component = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (component == null)
        {
            throw new ArgumentException($"Component item with ID {command.Id} not found.");
        }

        component.MarkAsUsed();

        _repository.Update(component);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
