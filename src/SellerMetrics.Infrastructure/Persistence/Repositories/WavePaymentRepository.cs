using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WavePayment.
/// </summary>
public class WavePaymentRepository : RepositoryBase<WavePayment>, IWavePaymentRepository
{
    public WavePaymentRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<WavePayment?> GetByWavePaymentIdAsync(
        string wavePaymentId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.WavePaymentId == wavePaymentId && p.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<WavePayment>> GetByInvoiceIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.WaveInvoiceId == invoiceId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WavePayment>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Invoice)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WavePayment>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Invoice)
            .Where(p => p.UserId == userId &&
                        p.PaymentDate >= startDate &&
                        p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalPaymentsAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.UserId == userId &&
                        p.PaymentDate >= startDate &&
                        p.PaymentDate <= endDate &&
                        p.Amount.Currency == currency)
            .SumAsync(p => p.Amount.Amount, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string wavePaymentId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.WavePaymentId == wavePaymentId && p.UserId == userId, cancellationToken);
    }

    public override async Task<WavePayment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<WavePayment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Invoice)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }
}
