using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ComponentType.
/// </summary>
public class ComponentTypeRepository : RepositoryBase<ComponentType>, IComponentTypeRepository
{
    public ComponentTypeRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<ComponentType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ct => ct.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<IReadOnlyList<ComponentType>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(ct => ct.SortOrder)
            .ThenBy(ct => ct.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(ct => ct.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(ct => ct.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
