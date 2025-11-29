using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Commands;

/// <summary>
/// Command to update an existing revenue entry.
/// </summary>
public record UpdateRevenueEntryCommand(
    int Id,
    RevenueSource Source,
    DateTime TransactionDate,
    string Description,
    decimal GrossAmount,
    decimal FeesAmount,
    decimal ShippingAmount,
    decimal TaxesCollectedAmount,
    string Currency = "USD",
    string? EbayOrderId = null,
    string? WaveInvoiceNumber = null,
    int? InventoryItemId = null,
    int? ServiceJobId = null,
    string? Notes = null);

/// <summary>
/// Handler for UpdateRevenueEntryCommand.
/// </summary>
public class UpdateRevenueEntryCommandHandler
{
    private readonly IRevenueEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRevenueEntryCommandHandler(
        IRevenueEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateRevenueEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (entry == null)
        {
            throw new InvalidOperationException($"Revenue entry with ID {command.Id} not found.");
        }

        // Check for duplicate eBay order (excluding current entry)
        if (!string.IsNullOrEmpty(command.EbayOrderId) && command.EbayOrderId != entry.EbayOrderId)
        {
            var existingEbayOrder = await _repository.GetByEbayOrderIdAsync(command.EbayOrderId, cancellationToken);
            if (existingEbayOrder != null && existingEbayOrder.Id != command.Id)
            {
                throw new InvalidOperationException($"Revenue entry for eBay order '{command.EbayOrderId}' already exists.");
            }
        }

        // Check for duplicate Wave invoice (excluding current entry)
        if (!string.IsNullOrEmpty(command.WaveInvoiceNumber) && command.WaveInvoiceNumber != entry.WaveInvoiceNumber)
        {
            var existingWaveInvoice = await _repository.GetByWaveInvoiceNumberAsync(command.WaveInvoiceNumber, cancellationToken);
            if (existingWaveInvoice != null && existingWaveInvoice.Id != command.Id)
            {
                throw new InvalidOperationException($"Revenue entry for Wave invoice '{command.WaveInvoiceNumber}' already exists.");
            }
        }

        entry.Source = command.Source;
        entry.TransactionDate = command.TransactionDate;
        entry.Description = command.Description;
        entry.GrossAmount = new Money(command.GrossAmount, command.Currency);
        entry.Fees = new Money(command.FeesAmount, command.Currency);
        entry.Shipping = new Money(command.ShippingAmount, command.Currency);
        entry.TaxesCollected = new Money(command.TaxesCollectedAmount, command.Currency);
        entry.EbayOrderId = command.EbayOrderId;
        entry.WaveInvoiceNumber = command.WaveInvoiceNumber;
        entry.InventoryItemId = command.InventoryItemId;
        entry.ServiceJobId = command.ServiceJobId;
        entry.Notes = command.Notes;
        entry.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
