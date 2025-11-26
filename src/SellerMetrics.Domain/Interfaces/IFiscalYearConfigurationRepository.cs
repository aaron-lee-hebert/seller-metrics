using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for fiscal year configuration operations.
/// </summary>
public interface IFiscalYearConfigurationRepository : IRepository<FiscalYearConfiguration>
{
    /// <summary>
    /// Gets the active fiscal year configuration.
    /// </summary>
    Task<FiscalYearConfiguration?> GetActiveAsync(CancellationToken cancellationToken = default);
}
