using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for ComponentType.
/// </summary>
public interface IComponentTypeRepository : IRepository<ComponentType>
{
    /// <summary>
    /// Gets a component type by name.
    /// </summary>
    Task<ComponentType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all component types ordered by sort order and name.
    /// </summary>
    Task<IReadOnlyList<ComponentType>> GetAllOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a component type name already exists.
    /// </summary>
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
