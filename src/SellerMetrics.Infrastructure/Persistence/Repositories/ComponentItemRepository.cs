using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ComponentItem.
/// </summary>
public class ComponentItemRepository : RepositoryBase<ComponentItem>, IComponentItemRepository
{
    public ComponentItemRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ComponentItem>> GetByTypeAsync(
        int componentTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Where(c => c.ComponentTypeId == componentTypeId)
            .OrderBy(c => c.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentItem>> GetByStatusAsync(
        ComponentStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Where(c => c.Status == status)
            .OrderBy(c => c.ComponentType.Name)
            .ThenBy(c => c.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentItem>> GetByLocationAsync(
        int storageLocationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Where(c => c.StorageLocationId == storageLocationId)
            .OrderBy(c => c.ComponentType.Name)
            .ThenBy(c => c.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentItem>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Where(c => c.ServiceJobId == serviceJobId)
            .OrderBy(c => c.ComponentType.Name)
            .ThenBy(c => c.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentItem>> GetLowStockAsync(
        int threshold = 1,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Where(c => c.Status == ComponentStatus.Available && c.Quantity <= threshold)
            .OrderBy(c => c.Quantity)
            .ThenBy(c => c.ComponentType.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalComponentValueAsync(
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Status == ComponentStatus.Available && c.UnitCost.Currency == currency)
            .SumAsync(c => c.UnitCost.Amount * c.Quantity, cancellationToken);
    }

    public async Task<ComponentItem?> GetWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Include(c => c.ServiceJob)
            .Include(c => c.Adjustments.OrderByDescending(a => a.AdjustedAt))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentItem>> GetExpiredDeletedAsync(
        int retentionDays = 30,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        return await _dbSet
            .IgnoreQueryFilters()
            .Where(c => c.IsDeleted && c.DeletedAt.HasValue && c.DeletedAt.Value < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public override async Task<ComponentItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .Include(c => c.ServiceJob)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<ComponentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ComponentType)
            .Include(c => c.StorageLocation)
            .OrderBy(c => c.ComponentType.Name)
            .ThenBy(c => c.Description)
            .ToListAsync(cancellationToken);
    }
}
