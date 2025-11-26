using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for EstimatedTaxPayment entities.
/// </summary>
public class EstimatedTaxPaymentRepository : RepositoryBase<EstimatedTaxPayment>, IEstimatedTaxPaymentRepository
{
    public EstimatedTaxPaymentRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EstimatedTaxPayment>> GetByTaxYearAsync(
        int taxYear,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.TaxYear == taxYear)
            .OrderBy(e => e.Quarter)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EstimatedTaxPayment?> GetByQuarterAsync(
        int taxYear,
        int quarter,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.TaxYear == taxYear && e.Quarter == quarter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EstimatedTaxPayment>> GetOverdueAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _dbSet
            .Where(e => !e.IsPaid && e.DueDate < today)
            .OrderBy(e => e.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EstimatedTaxPayment>> GetUnpaidAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsPaid)
            .OrderBy(e => e.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EstimatedTaxPayment?> GetNextUpcomingAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _dbSet
            .Where(e => !e.IsPaid && e.DueDate >= today)
            .OrderBy(e => e.DueDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
