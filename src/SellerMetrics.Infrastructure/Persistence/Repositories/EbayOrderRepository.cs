using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for EbayOrder.
/// </summary>
public class EbayOrderRepository : RepositoryBase<EbayOrder>, IEbayOrderRepository
{
    public EbayOrderRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<EbayOrder?> GetByEbayOrderIdAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.InventoryItem)
            .FirstOrDefaultAsync(o => o.EbayOrderId == ebayOrderId && o.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.UserId == userId &&
                        o.OrderDate >= startDate &&
                        o.OrderDate <= endDate)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetByStatusAsync(
        string userId,
        EbayOrderStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.UserId == userId && o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetByInventoryItemIdAsync(
        int inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.InventoryItemId == inventoryItemId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetUnlinkedOrdersAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.InventoryItemId == null)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalGrossSalesAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.UserId == userId &&
                        o.OrderDate >= startDate &&
                        o.OrderDate <= endDate &&
                        o.GrossSale.Currency == currency)
            .SumAsync(o => o.GrossSale.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalProfitAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        var orders = await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.UserId == userId &&
                        o.OrderDate >= startDate &&
                        o.OrderDate <= endDate &&
                        o.InventoryItemId != null &&
                        o.GrossSale.Currency == currency)
            .ToListAsync(cancellationToken);

        return orders
            .Where(o => o.Profit != null)
            .Sum(o => o.Profit!.Amount);
    }

    public async Task<int> GetOrderCountAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(o => o.UserId == userId &&
                            o.OrderDate >= startDate &&
                            o.OrderDate <= endDate, cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrder>> GetRecentOrdersAsync(
        string userId,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(o => o.EbayOrderId == ebayOrderId && o.UserId == userId, cancellationToken);
    }

    public async Task<bool> WasDeletedAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(o => o.EbayOrderId == ebayOrderId && o.UserId == userId && o.IsDeleted, cancellationToken);
    }

    public override async Task<EbayOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.InventoryItem)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<EbayOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(o => o.InventoryItem)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }
}
