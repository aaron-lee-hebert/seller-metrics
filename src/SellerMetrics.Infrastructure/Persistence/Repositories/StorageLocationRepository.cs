using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for StorageLocation with hierarchy-specific operations.
/// </summary>
public class StorageLocationRepository : RepositoryBase<StorageLocation>, IStorageLocationRepository
{
    public StorageLocationRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<StorageLocation>> GetRootLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(sl => sl.ParentId == null)
            .OrderBy(sl => sl.SortOrder)
            .ThenBy(sl => sl.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<StorageLocation?> GetWithChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        // Load the location and recursively include all descendants
        var location = await _dbSet
            .Include(sl => sl.Children)
            .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);

        if (location != null)
        {
            await LoadChildrenRecursivelyAsync(location, cancellationToken);
        }

        return location;
    }

    public async Task<IReadOnlyList<StorageLocation>> GetHierarchyAsync(CancellationToken cancellationToken = default)
    {
        // Load all locations and build hierarchy in memory
        var allLocations = await _dbSet
            .AsNoTracking()
            .OrderBy(sl => sl.SortOrder)
            .ThenBy(sl => sl.Name)
            .ToListAsync(cancellationToken);

        // Build hierarchy: assign children to their parents
        var locationDict = allLocations.ToDictionary(sl => sl.Id);
        foreach (var location in allLocations)
        {
            if (location.ParentId.HasValue && locationDict.TryGetValue(location.ParentId.Value, out var parent))
            {
                parent.Children.Add(location);
                location.Parent = parent;
            }
        }

        // Return only root locations (children are populated)
        return allLocations.Where(sl => sl.ParentId == null).ToList();
    }

    public async Task<StorageLocation?> GetWithAncestorsAsync(int id, CancellationToken cancellationToken = default)
    {
        var location = await _dbSet
            .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);

        if (location != null)
        {
            await LoadAncestorsRecursivelyAsync(location, cancellationToken);
        }

        return location;
    }

    public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(sl => sl.ParentId == id, cancellationToken);
    }

    public async Task<bool> HasItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        // Check for InventoryItems at this location
        var hasInventory = await _context.Set<InventoryItem>()
            .AnyAsync(i => i.StorageLocationId == id, cancellationToken);

        if (hasInventory)
            return true;

        // Check for ComponentItems at this location
        var hasComponents = await _context.Set<ComponentItem>()
            .AnyAsync(c => c.StorageLocationId == id, cancellationToken);

        return hasComponents;
    }

    public async Task<IReadOnlyList<StorageLocation>> GetExpiredDeletedAsync(int retentionDays = 30, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        return await _dbSet
            .IgnoreQueryFilters()
            .Where(sl => sl.IsDeleted && sl.DeletedAt.HasValue && sl.DeletedAt.Value < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    private async Task LoadChildrenRecursivelyAsync(StorageLocation location, CancellationToken cancellationToken)
    {
        await _context.Entry(location)
            .Collection(sl => sl.Children)
            .LoadAsync(cancellationToken);

        foreach (var child in location.Children)
        {
            await LoadChildrenRecursivelyAsync(child, cancellationToken);
        }
    }

    private async Task LoadAncestorsRecursivelyAsync(StorageLocation location, CancellationToken cancellationToken)
    {
        if (location.ParentId.HasValue)
        {
            await _context.Entry(location)
                .Reference(sl => sl.Parent)
                .LoadAsync(cancellationToken);

            if (location.Parent != null)
            {
                await LoadAncestorsRecursivelyAsync(location.Parent, cancellationToken);
            }
        }
    }
}
