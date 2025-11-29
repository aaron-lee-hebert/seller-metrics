using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Revenue.Commands;

/// <summary>
/// Command to create a new revenue entry.
/// </summary>
public record CreateRevenueEntryCommand(
    RevenueSource Source,
    DateTime TransactionDate,
    string Description,
    decimal GrossAmount,
    decimal FeesAmount = 0,
    decimal ShippingAmount = 0,
    decimal TaxesCollectedAmount = 0,
    string Currency = "USD",
    string? EbayOrderId = null,
    string? WaveInvoiceNumber = null,
    int? InventoryItemId = null,
    int? ServiceJobId = null,
    string? Notes = null);

/// <summary>
/// Handler for CreateRevenueEntryCommand.
/// </summary>
public class CreateRevenueEntryCommandHandler
{
    private readonly IRevenueEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRevenueEntryCommandHandler(
        IRevenueEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(
        CreateRevenueEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        // Check for duplicate eBay order
        if (!string.IsNullOrEmpty(command.EbayOrderId))
        {
            var existingEbayOrder = await _repository.GetByEbayOrderIdAsync(command.EbayOrderId, cancellationToken);
            if (existingEbayOrder != null)
            {
                throw new InvalidOperationException($"Revenue entry for eBay order '{command.EbayOrderId}' already exists.");
            }
        }

        // Check for duplicate Wave invoice
        if (!string.IsNullOrEmpty(command.WaveInvoiceNumber))
        {
            var existingWaveInvoice = await _repository.GetByWaveInvoiceNumberAsync(command.WaveInvoiceNumber, cancellationToken);
            if (existingWaveInvoice != null)
            {
                throw new InvalidOperationException($"Revenue entry for Wave invoice '{command.WaveInvoiceNumber}' already exists.");
            }
        }

        var entry = new RevenueEntry
        {
            Source = command.Source,
            EntryType = RevenueEntryType.Manual,
            TransactionDate = command.TransactionDate,
            Description = command.Description,
            GrossAmount = new Money(command.GrossAmount, command.Currency),
            Fees = new Money(command.FeesAmount, command.Currency),
            Shipping = new Money(command.ShippingAmount, command.Currency),
            TaxesCollected = new Money(command.TaxesCollectedAmount, command.Currency),
            EbayOrderId = command.EbayOrderId,
            WaveInvoiceNumber = command.WaveInvoiceNumber,
            InventoryItemId = command.InventoryItemId,
            ServiceJobId = command.ServiceJobId,
            Notes = command.Notes
        };

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
