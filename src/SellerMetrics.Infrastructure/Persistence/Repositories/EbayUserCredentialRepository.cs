using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for EbayUserCredential.
/// </summary>
public class EbayUserCredentialRepository : RepositoryBase<EbayUserCredential>, IEbayUserCredentialRepository
{
    public EbayUserCredentialRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<EbayUserCredential?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<EbayUserCredential>> GetAllConnectedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.IsConnected)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayUserCredential>> GetNeedingTokenRefreshAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(c => c.IsConnected &&
                        c.AccessTokenExpiresAt <= now &&
                        c.RefreshTokenExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EbayUserCredential>> GetWithExpiredRefreshTokensAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Where(c => c.IsConnected && c.RefreshTokenExpiresAt <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserConnectedAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.UserId == userId && c.IsConnected, cancellationToken);
    }
}
