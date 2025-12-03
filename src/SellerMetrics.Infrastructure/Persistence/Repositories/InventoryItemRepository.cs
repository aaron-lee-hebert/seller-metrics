using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for InventoryItem.
/// </summary>
public class InventoryItemRepository : RepositoryBase<InventoryItem>, IInventoryItemRepository
{
    public InventoryItemRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByStatusAsync(
        InventoryStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.StorageLocation)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByLocationAsync(
        int storageLocationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.StorageLocation)
            .Where(i => i.StorageLocationId == storageLocationId)
            .OrderBy(i => i.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> GetByInternalSkuAsync(
        string internalSku,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.StorageLocation)
            .FirstOrDefaultAsync(i => i.InternalSku == internalSku, cancellationToken);
    }

    public async Task<InventoryItem?> GetByEbaySkuAsync(
        string ebaySku,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.StorageLocation)
            .FirstOrDefaultAsync(i => i.EbaySku == ebaySku, cancellationToken);
    }

    public async Task<InventoryItem?> GetByEbayListingIdAsync(
        string ebayListingId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.StorageLocation)
            .FirstOrDefaultAsync(i => i.EbayListingId == ebayListingId, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = searchTerm.ToLowerInvariant();

        return await _dbSet
            .AsNoTracking()
            .Include(i => i.StorageLocation)
            .Where(i =>
                i.Title.ToLower().Contains(normalizedSearch) ||
                i.InternalSku.ToLower().Contains(normalizedSearch) ||
                (i.EbaySku != null && i.EbaySku.ToLower().Contains(normalizedSearch)) ||
                (i.Description != null && i.Description.ToLower().Contains(normalizedSearch)) ||
                (i.Notes != null && i.Notes.ToLower().Contains(normalizedSearch)))
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalInventoryValueAsync(
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Status != InventoryStatus.Sold && i.Cogs.Currency == currency)
            .SumAsync(i => i.Cogs.Amount * i.Quantity, cancellationToken);
    }

    public async Task<int> GetNextSkuSequenceAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"INV-{today:yyyyMMdd}-";

        var lastSku = await _dbSet
            .IgnoreQueryFilters()
            .Where(i => i.InternalSku.StartsWith(prefix))
            .OrderByDescending(i => i.InternalSku)
            .Select(i => i.InternalSku)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastSku == null)
            return 1;

        // Extract sequence number from SKU (format: INV-YYYYMMDD-XXXX)
        var sequencePart = lastSku.Substring(prefix.Length);
        if (int.TryParse(sequencePart, out var lastSequence))
            return lastSequence + 1;

        return 1;
    }

    public async Task<bool> InternalSkuExistsAsync(
        string internalSku,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(i => i.InternalSku == internalSku, cancellationToken);
    }

    public async Task<bool> EbaySkuExistsAsync(
        string ebaySku,
        int? excludeItemId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.IgnoreQueryFilters().Where(i => i.EbaySku == ebaySku);

        if (excludeItemId.HasValue)
            query = query.Where(i => i.Id != excludeItemId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetExpiredDeletedAsync(
        int retentionDays = 30,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        return await _dbSet
            .IgnoreQueryFilters()
            .Where(i => i.IsDeleted && i.DeletedAt.HasValue && i.DeletedAt.Value < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public override async Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.StorageLocation)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.StorageLocation)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
