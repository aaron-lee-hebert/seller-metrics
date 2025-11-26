using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for IRS mileage rate operations.
/// </summary>
public class IrsMileageRateRepository : RepositoryBase<IrsMileageRate>, IIrsMileageRateRepository
{
    public IrsMileageRateRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<IrsMileageRate?> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Year == year)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IrsMileageRate?> GetByDateAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        // Get the rate that was effective on the given date
        return await _dbSet
            .Where(r => r.EffectiveDate <= date)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IrsMileageRate>> GetAllOrderedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(r => r.Year)
            .ThenByDescending(r => r.EffectiveDate)
            .ToListAsync(cancellationToken);
    }
}
