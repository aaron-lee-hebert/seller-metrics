using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for mileage entry operations.
/// </summary>
public class MileageEntryRepository : RepositoryBase<MileageEntry>, IMileageEntryRepository
{
    public MileageEntryRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public override async Task<MileageEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.ServiceJob)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<MileageEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.ServiceJob)
            .OrderByDescending(m => m.TripDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MileageEntry>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.ServiceJob)
            .Where(m => m.TripDate >= startDate && m.TripDate <= endDate)
            .OrderByDescending(m => m.TripDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MileageEntry>> GetByBusinessLineAsync(
        BusinessLine businessLine,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.ServiceJob)
            .Where(m => m.BusinessLine == businessLine)
            .OrderByDescending(m => m.TripDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MileageEntry>> GetByBusinessLineAndDateRangeAsync(
        BusinessLine businessLine,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.ServiceJob)
            .Where(m => m.BusinessLine == businessLine &&
                        m.TripDate >= startDate &&
                        m.TripDate <= endDate)
            .OrderByDescending(m => m.TripDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MileageEntry>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.ServiceJob)
            .Where(m => m.ServiceJobId == serviceJobId)
            .OrderByDescending(m => m.TripDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalMilesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var entries = await _dbSet
            .Where(m => m.TripDate >= startDate && m.TripDate <= endDate)
            .ToListAsync(cancellationToken);

        return entries.Sum(m => m.TotalMiles);
    }

    public async Task<decimal> GetTotalMilesByBusinessLineAsync(
        BusinessLine businessLine,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var entries = await _dbSet
            .Where(m => m.BusinessLine == businessLine &&
                        m.TripDate >= startDate &&
                        m.TripDate <= endDate)
            .ToListAsync(cancellationToken);

        return entries.Sum(m => m.TotalMiles);
    }
}
