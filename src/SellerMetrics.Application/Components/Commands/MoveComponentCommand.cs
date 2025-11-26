using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to move a component to a different storage location.
/// </summary>
public record MoveComponentCommand(int Id, int? NewStorageLocationId);

/// <summary>
/// Handler for MoveComponentCommand.
/// </summary>
public class MoveComponentCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveComponentCommandHandler(
        IComponentItemRepository repository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        MoveComponentCommand command,
        CancellationToken cancellationToken = default)
    {
        var component = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (component == null)
        {
            throw new ArgumentException($"Component item with ID {command.Id} not found.");
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

        component.MoveTo(command.NewStorageLocationId);

        _repository.Update(component);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
