using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for fiscal year configuration operations.
/// </summary>
public class FiscalYearConfigurationRepository : RepositoryBase<FiscalYearConfiguration>, IFiscalYearConfigurationRepository
{
    public FiscalYearConfigurationRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<FiscalYearConfiguration?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f => f.IsActive, cancellationToken);
    }
}
