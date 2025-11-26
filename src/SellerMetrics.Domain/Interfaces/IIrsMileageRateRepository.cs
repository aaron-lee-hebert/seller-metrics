using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for IRS mileage rate operations.
/// </summary>
public interface IIrsMileageRateRepository : IRepository<IrsMileageRate>
{
    /// <summary>
    /// Gets the rate for a specific year.
    /// </summary>
    Task<IrsMileageRate?> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the rate applicable for a specific date.
    /// Handles mid-year rate changes.
    /// </summary>
    Task<IrsMileageRate?> GetByDateAsync(
        DateTime date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all rates ordered by year (descending).
    /// </summary>
    Task<IReadOnlyList<IrsMileageRate>> GetAllOrderedAsync(
        CancellationToken cancellationToken = default);
}
