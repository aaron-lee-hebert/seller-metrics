using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WaveInvoice.
/// </summary>
public class WaveInvoiceRepository : RepositoryBase<WaveInvoice>, IWaveInvoiceRepository
{
    public WaveInvoiceRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<WaveInvoice?> GetByWaveInvoiceIdAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.WaveInvoiceId == waveInvoiceId && i.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId &&
                        i.InvoiceDate >= startDate &&
                        i.InvoiceDate <= endDate)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetByStatusAsync(
        string userId,
        WaveInvoiceStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId && i.Status == status)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetUnpaidInvoicesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId &&
                        i.Status != WaveInvoiceStatus.Paid &&
                        i.Status != WaveInvoiceStatus.Voided)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetOverdueInvoicesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId &&
                        i.Status != WaveInvoiceStatus.Paid &&
                        i.Status != WaveInvoiceStatus.Voided &&
                        i.DueDate < today)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.UserId == userId &&
                        i.Status == WaveInvoiceStatus.Paid &&
                        i.InvoiceDate >= startDate &&
                        i.InvoiceDate <= endDate &&
                        i.TotalAmount.Currency == currency)
            .SumAsync(i => i.TotalAmount.Amount, cancellationToken);
    }

    public async Task<int> GetInvoiceCountAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(i => i.UserId == userId &&
                            i.InvoiceDate >= startDate &&
                            i.InvoiceDate <= endDate, cancellationToken);
    }

    public async Task<IReadOnlyList<WaveInvoice>> GetRecentInvoicesAsync(
        string userId,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.InvoiceDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(i => i.WaveInvoiceId == waveInvoiceId && i.UserId == userId, cancellationToken);
    }

    public async Task<bool> WasDeletedAsync(
        string waveInvoiceId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(i => i.WaveInvoiceId == waveInvoiceId && i.UserId == userId && i.IsDeleted, cancellationToken);
    }

    public override async Task<WaveInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<WaveInvoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Payments)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }
}
