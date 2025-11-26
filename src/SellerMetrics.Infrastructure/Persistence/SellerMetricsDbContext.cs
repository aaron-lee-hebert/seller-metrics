using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence;

/// <summary>
/// The main database context for SellerMetrics application.
/// Inherits from IdentityDbContext to support ASP.NET Core Identity.
/// </summary>
public class SellerMetricsDbContext : IdentityDbContext<ApplicationUser>
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

        // Configure Identity table names (optional - use default AspNet prefix)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
        });
    }
}
