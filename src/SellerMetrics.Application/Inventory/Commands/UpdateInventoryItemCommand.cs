using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to update an existing inventory item.
/// </summary>
public record UpdateInventoryItemCommand(
    int Id,
    string Title,
    string? Description,
    decimal CogsAmount,
    string CogsCurrency,
    DateTime? PurchaseDate,
    int? StorageLocationId,
    EbayCondition? Condition,
    string? Notes,
    string? EbaySku = null);

/// <summary>
/// Handler for UpdateInventoryItemCommand.
/// </summary>
public class UpdateInventoryItemCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryItemDto> HandleAsync(
        UpdateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Inventory item with ID {command.Id} not found.");
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

        // Check for eBay SKU conflict if changed
        if (!string.IsNullOrEmpty(command.EbaySku) && command.EbaySku != item.EbaySku)
        {
            if (await _repository.EbaySkuExistsAsync(command.EbaySku, command.Id, cancellationToken))
            {
                throw new InvalidOperationException($"eBay SKU '{command.EbaySku}' already exists.");
            }
        }

        item.Title = command.Title;
        item.Description = command.Description;
        item.Cogs = new Money(command.CogsAmount, command.CogsCurrency);
        item.PurchaseDate = command.PurchaseDate;
        item.StorageLocationId = command.StorageLocationId;
        item.StorageLocation = storageLocation;
        item.Condition = command.Condition;
        item.Notes = command.Notes;
        item.EbaySku = command.EbaySku;
        item.UpdatedAt = DateTime.UtcNow;

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(item);
    }

    private static InventoryItemDto MapToDto(InventoryItem item)
    {
        return new InventoryItemDto
        {
            Id = item.Id,
            InternalSku = item.InternalSku,
            EbaySku = item.EbaySku,
            EffectiveSku = item.EffectiveSku,
            Title = item.Title,
            Description = item.Description,
            CogsAmount = item.Cogs.Amount,
            CogsCurrency = item.Cogs.Currency,
            CogsFormatted = item.Cogs.ToString(),
            PurchaseDate = item.PurchaseDate,
            StorageLocationId = item.StorageLocationId,
            StorageLocationPath = item.StorageLocation?.FullPath,
            Status = item.Status,
            StatusDisplay = item.Status.ToString(),
            Condition = item.Condition,
            ConditionDisplay = item.Condition?.GetDisplayName(),
            Notes = item.Notes,
            PhotoPath = item.PhotoPath,
            EbayListingId = item.EbayListingId,
            ListedDate = item.ListedDate,
            SoldDate = item.SoldDate,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
