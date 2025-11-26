using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to create a new inventory item.
/// </summary>
public record CreateInventoryItemCommand(
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
/// Handler for CreateInventoryItemCommand.
/// </summary>
public class CreateInventoryItemCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryItemDto> HandleAsync(
        CreateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
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

        // Generate internal SKU
        var sequence = await _repository.GetNextSkuSequenceAsync(cancellationToken);
        var internalSku = InventoryItem.GenerateInternalSku(sequence);

        // Check for eBay SKU conflict - if provided eBay SKU matches an existing internal SKU, use eBay SKU
        string? finalEbaySku = command.EbaySku;
        if (!string.IsNullOrEmpty(command.EbaySku))
        {
            // Check if eBay SKU already exists
            if (await _repository.EbaySkuExistsAsync(command.EbaySku, null, cancellationToken))
            {
                throw new InvalidOperationException($"eBay SKU '{command.EbaySku}' already exists.");
            }
        }

        var item = new InventoryItem
        {
            InternalSku = internalSku,
            EbaySku = finalEbaySku,
            Title = command.Title,
            Description = command.Description,
            Cogs = new Money(command.CogsAmount, command.CogsCurrency),
            PurchaseDate = command.PurchaseDate,
            StorageLocationId = command.StorageLocationId,
            StorageLocation = storageLocation,
            Condition = command.Condition,
            Notes = command.Notes,
            Status = InventoryStatus.Unlisted
        };

        await _repository.AddAsync(item, cancellationToken);
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
