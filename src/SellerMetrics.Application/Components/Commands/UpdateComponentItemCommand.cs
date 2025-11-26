using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Components.Commands;

/// <summary>
/// Command to update an existing component item.
/// </summary>
public record UpdateComponentItemCommand(
    int Id,
    int ComponentTypeId,
    string Description,
    decimal UnitCostAmount,
    string UnitCostCurrency,
    int? StorageLocationId,
    DateTime? AcquiredDate,
    ComponentSource Source,
    string? Notes);

/// <summary>
/// Handler for UpdateComponentItemCommand.
/// </summary>
public class UpdateComponentItemCommandHandler
{
    private readonly IComponentItemRepository _repository;
    private readonly IComponentTypeRepository _componentTypeRepository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateComponentItemCommandHandler(
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
        UpdateComponentItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var component = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (component == null)
        {
            throw new ArgumentException($"Component item with ID {command.Id} not found.");
        }

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

        component.ComponentTypeId = command.ComponentTypeId;
        component.ComponentType = componentType;
        component.Description = command.Description;
        component.UnitCost = new Money(command.UnitCostAmount, command.UnitCostCurrency);
        component.StorageLocationId = command.StorageLocationId;
        component.StorageLocation = storageLocation;
        component.AcquiredDate = command.AcquiredDate;
        component.Source = command.Source;
        component.Notes = command.Notes;
        component.UpdatedAt = DateTime.UtcNow;

        _repository.Update(component);
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
