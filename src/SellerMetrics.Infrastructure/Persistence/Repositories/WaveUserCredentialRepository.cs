using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WaveUserCredential.
/// </summary>
public class WaveUserCredentialRepository : RepositoryBase<WaveUserCredential>, IWaveUserCredentialRepository
{
    public WaveUserCredentialRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<WaveUserCredential?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<WaveUserCredential>> GetAllConnectedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.IsConnected)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.UserId == userId, cancellationToken);
    }
}
