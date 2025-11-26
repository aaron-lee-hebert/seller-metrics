using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for coordinating database transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SellerMetricsDbContext _context;
    private bool _disposed;

    public UnitOfWork(SellerMetricsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}
