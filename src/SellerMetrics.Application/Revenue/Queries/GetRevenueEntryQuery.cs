using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get a single revenue entry by ID.
/// </summary>
public record GetRevenueEntryQuery(int Id);

/// <summary>
/// Handler for GetRevenueEntryQuery.
/// </summary>
public class GetRevenueEntryQueryHandler
{
    private readonly IRevenueEntryRepository _repository;

    public GetRevenueEntryQueryHandler(IRevenueEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<RevenueEntryDto?> HandleAsync(
        GetRevenueEntryQuery query,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (entry == null)
        {
            return null;
        }

        return MapToDto(entry);
    }

    private static RevenueEntryDto MapToDto(RevenueEntry entry)
    {
        return new RevenueEntryDto
        {
            Id = entry.Id,
            Source = entry.Source,
            SourceDisplay = entry.Source.ToString(),
            EntryType = entry.EntryType,
            EntryTypeDisplay = entry.EntryType.ToString(),
            TransactionDate = entry.TransactionDate,
            Description = entry.Description,
            GrossAmount = entry.GrossAmount.Amount,
            FeesAmount = entry.Fees.Amount,
            NetAmount = entry.NetAmount.Amount,
            Currency = entry.GrossAmount.Currency,
            GrossFormatted = entry.GrossAmount.ToString(),
            FeesFormatted = entry.Fees.ToString(),
            NetFormatted = entry.NetAmount.ToString(),
            EbayOrderId = entry.EbayOrderId,
            WaveInvoiceNumber = entry.WaveInvoiceNumber,
            InventoryItemId = entry.InventoryItemId,
            InventoryItemSku = entry.InventoryItem?.EffectiveSku,
            ServiceJobId = entry.ServiceJobId,
            ServiceJobNumber = entry.ServiceJob?.JobNumber,
            Notes = entry.Notes,
            LastSyncedAt = entry.LastSyncedAt,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
    }
}
