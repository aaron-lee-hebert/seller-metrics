using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Inventory.Commands;

/// <summary>
/// Command to sell one unit of an inventory item.
/// If quantity > 1, decrements quantity and creates a new sold record.
/// If quantity = 1, marks the item as sold.
/// </summary>
public record SellInventoryItemCommand(int Id);

/// <summary>
/// Result of selling an inventory item.
/// </summary>
public class SellInventoryItemResult
{
    /// <summary>
    /// The original item (updated with decremented quantity or marked as sold).
    /// </summary>
    public InventoryItemDto OriginalItem { get; set; } = null!;

    /// <summary>
    /// The sold item record (only populated if quantity was > 1).
    /// </summary>
    public InventoryItemDto? SoldItem { get; set; }

    /// <summary>
    /// True if the original item was completely sold (quantity was 1).
    /// </summary>
    public bool IsCompletelySold { get; set; }
}

/// <summary>
/// Handler for SellInventoryItemCommand.
/// </summary>
public class SellInventoryItemCommandHandler
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SellInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SellInventoryItemResult> HandleAsync(
        SellInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Inventory item with ID {command.Id} not found.");
        }

        // SellOne handles all validation and logic
        var soldItem = item.SellOne();

        _repository.Update(item);

        // If a new sold record was created, add it to the repository
        if (soldItem != null)
        {
            await _repository.AddAsync(soldItem, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SellInventoryItemResult
        {
            OriginalItem = MapToDto(item),
            SoldItem = soldItem != null ? MapToDto(soldItem) : null,
            IsCompletelySold = soldItem == null
        };
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
            Quantity = item.Quantity,
            TotalValueAmount = item.TotalValue.Amount,
            TotalValueFormatted = item.TotalValue.ToString(),
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
