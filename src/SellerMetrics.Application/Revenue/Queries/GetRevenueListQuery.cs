using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Revenue.Queries;

/// <summary>
/// Query to get revenue entries with optional filters.
/// </summary>
public record GetRevenueListQuery(
    RevenueSource? Source = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null);

/// <summary>
/// Handler for GetRevenueListQuery.
/// </summary>
public class GetRevenueListQueryHandler
{
    private readonly IRevenueEntryRepository _repository;

    public GetRevenueListQueryHandler(IRevenueEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RevenueEntryDto>> HandleAsync(
        GetRevenueListQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RevenueEntry> entries;

        if (query.Source.HasValue && query.StartDate.HasValue && query.EndDate.HasValue)
        {
            entries = await _repository.GetBySourceAndDateRangeAsync(
                query.Source.Value,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            entries = await _repository.GetByDateRangeAsync(
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else if (query.Source.HasValue)
        {
            entries = await _repository.GetBySourceAsync(query.Source.Value, cancellationToken);
        }
        else
        {
            entries = await _repository.GetAllAsync(cancellationToken);
        }

        return entries.Select(MapToDto).ToList();
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
            ShippingAmount = entry.Shipping.Amount,
            TaxesCollectedAmount = entry.TaxesCollected.Amount,
            NetAmount = entry.NetAmount.Amount,
            Currency = entry.GrossAmount.Currency,
            GrossFormatted = entry.GrossAmount.ToString(),
            FeesFormatted = entry.Fees.ToString(),
            ShippingFormatted = entry.Shipping.ToString(),
            TaxesCollectedFormatted = entry.TaxesCollected.ToString(),
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
