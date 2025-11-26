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

    /// <summary>
    /// Storage locations for inventory and components.
    /// </summary>
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();

    /// <summary>
    /// eBay inventory items.
    /// </summary>
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    /// <summary>
    /// Computer repair component items.
    /// </summary>
    public DbSet<ComponentItem> ComponentItems => Set<ComponentItem>();

    /// <summary>
    /// Component types (catalog of part categories).
    /// </summary>
    public DbSet<ComponentType> ComponentTypes => Set<ComponentType>();

    /// <summary>
    /// Component quantity adjustment audit records.
    /// </summary>
    public DbSet<ComponentQuantityAdjustment> ComponentQuantityAdjustments => Set<ComponentQuantityAdjustment>();

    /// <summary>
    /// Service/repair jobs.
    /// </summary>
    public DbSet<ServiceJob> ServiceJobs => Set<ServiceJob>();

    /// <summary>
    /// Revenue entries from eBay and services.
    /// </summary>
    public DbSet<RevenueEntry> RevenueEntries => Set<RevenueEntry>();

    /// <summary>
    /// Fiscal year configuration settings.
    /// </summary>
    public DbSet<FiscalYearConfiguration> FiscalYearConfigurations => Set<FiscalYearConfiguration>();

    /// <summary>
    /// Business expenses for tax reporting.
    /// </summary>
    public DbSet<BusinessExpense> BusinessExpenses => Set<BusinessExpense>();

    /// <summary>
    /// Mileage log entries for tax deductions.
    /// </summary>
    public DbSet<MileageEntry> MileageEntries => Set<MileageEntry>();

    /// <summary>
    /// IRS mileage rates by year.
    /// </summary>
    public DbSet<IrsMileageRate> IrsMileageRates => Set<IrsMileageRate>();

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
