using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to create a new component item.
/// </summary>
public record CreateComponentItemCommand(
    int ComponentTypeId,
    string Description,
    int Quantity,
    decimal UnitCostAmount,
    string UnitCostCurrency,
    int? StorageLocationId,
    DateTime? AcquiredDate,
    ComponentSource Source,
    string? Notes);

/// <summary>
/// Handler for CreateComponentItemCommand.
/// </summary>
public class CreateComponentItemCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IComponentTypeRepository _componentTypeRepository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateComponentItemCommandHandler(
        IComponentItemRepository repository,
        IComponentTypeRepository componentTypeRepository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _componentTypeRepository = componentTypeRepository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ComponentItemDto> HandleAsync(
        CreateComponentItemCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate component type exists
        var componentType = await _componentTypeRepository.GetByIdAsync(
            command.ComponentTypeId, cancellationToken);
        if (componentType == null)
        {
            throw new ArgumentException($"Component type with ID {command.ComponentTypeId} not found.");
        }

        // Validate storage location exists if specified
        StorageLocation? storageLocation = null;
        if (command.StorageLocationId.HasValue)
        {
            storageLocation = await _storageLocationRepository.GetWithAncestorsAsync(
                command.StorageLocationId.Value, cancellationToken);
            if (storageLocation == null)
            {
                throw new ArgumentException($"Storage location with ID {command.StorageLocationId} not found.");
            }
        }

        if (command.Quantity < 1)
        {
            throw new ArgumentException("Quantity must be at least 1.");
        }

        var component = new ComponentItem
        {
            ComponentTypeId = command.ComponentTypeId,
            ComponentType = componentType,
            Description = command.Description,
            Quantity = command.Quantity,
            UnitCost = new Money(command.UnitCostAmount, command.UnitCostCurrency),
            StorageLocationId = command.StorageLocationId,
            StorageLocation = storageLocation,
            AcquiredDate = command.AcquiredDate,
            Source = command.Source,
            Notes = command.Notes,
            Status = ComponentStatus.Available
        };

        await _repository.AddAsync(component, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(component);
    }

    private static ComponentItemDto MapToDto(ComponentItem item)
    {
        return new ComponentItemDto
        {
            Id = item.Id,
            ComponentTypeId = item.ComponentTypeId,
            ComponentTypeName = item.ComponentType?.Name ?? string.Empty,
            Description = item.Description,
            Quantity = item.Quantity,
            UnitCostAmount = item.UnitCost.Amount,
            UnitCostCurrency = item.UnitCost.Currency,
            UnitCostFormatted = item.UnitCost.ToString(),
            TotalValueAmount = item.TotalValue.Amount,
            TotalValueFormatted = item.TotalValue.ToString(),
            StorageLocationId = item.StorageLocationId,
            StorageLocationPath = item.StorageLocation?.FullPath,
            Status = item.Status,
            StatusDisplay = item.Status.ToString(),
            AcquiredDate = item.AcquiredDate,
            Source = item.Source,
            SourceDisplay = item.Source.ToString(),
            Notes = item.Notes,
            ServiceJobId = item.ServiceJobId,
            ServiceJobNumber = item.ServiceJob?.JobNumber,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
