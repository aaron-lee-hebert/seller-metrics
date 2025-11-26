using Microsoft.EntityFrameworkCore;

namespace SellerMetrics.Infrastructure.Persistence;

/// <summary>
/// The main database context for SellerMetrics application.
/// </summary>
public class SellerMetricsDbContext : DbContext
{
    public SellerMetricsDbContext(DbContextOptions<SellerMetricsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SellerMetricsDbContext).Assembly);
    }
}
