using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for revenue entry operations.
/// </summary>
public class RevenueEntryRepository : RepositoryBase<RevenueEntry>, IRevenueEntryRepository
{
    public RevenueEntryRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public override async Task<RevenueEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.InventoryItem)
            .Include(r => r.ServiceJob)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RevenueEntry>> GetBySourceAsync(
        RevenueSource source,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.InventoryItem)
            .Include(r => r.ServiceJob)
            .Where(r => r.Source == source)
            .OrderByDescending(r => r.TransactionDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RevenueEntry>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.InventoryItem)
            .Include(r => r.ServiceJob)
            .Where(r => r.TransactionDate >= startDate && r.TransactionDate <= endDate)
            .OrderByDescending(r => r.TransactionDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RevenueEntry>> GetBySourceAndDateRangeAsync(
        RevenueSource source,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.InventoryItem)
            .Include(r => r.ServiceJob)
            .Where(r => r.Source == source &&
                        r.TransactionDate >= startDate &&
                        r.TransactionDate <= endDate)
            .OrderByDescending(r => r.TransactionDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<RevenueEntry?> GetByEbayOrderIdAsync(
        string ebayOrderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.InventoryItem)
            .FirstOrDefaultAsync(r => r.EbayOrderId == ebayOrderId, cancellationToken);
    }

    public async Task<RevenueEntry?> GetByWaveInvoiceNumberAsync(
        string waveInvoiceNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.ServiceJob)
            .FirstOrDefaultAsync(r => r.WaveInvoiceNumber == waveInvoiceNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<RevenueEntry>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.ServiceJob)
            .Where(r => r.ServiceJobId == serviceJobId)
            .OrderByDescending(r => r.TransactionDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.TransactionDate >= startDate &&
                        r.TransactionDate <= endDate &&
                        r.GrossAmount.Currency == currency)
            .SumAsync(r => r.GrossAmount.Amount - r.Fees.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueBySourceAsync(
        RevenueSource source,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Source == source &&
                        r.TransactionDate >= startDate &&
                        r.TransactionDate <= endDate &&
                        r.GrossAmount.Currency == currency)
            .SumAsync(r => r.GrossAmount.Amount - r.Fees.Amount, cancellationToken);
    }

    public async Task<IReadOnlyList<(int Year, int Month, decimal Total)>> GetMonthlyRevenueTotalsAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        var results = await _dbSet
            .Where(r => r.TransactionDate >= startDate &&
                        r.TransactionDate <= endDate &&
                        r.GrossAmount.Currency == currency)
            .GroupBy(r => new { r.TransactionDate.Year, r.TransactionDate.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Sum(r => r.GrossAmount.Amount - r.Fees.Amount)
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Year, r.Month, r.Total)).ToList();
    }
}
