using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for EstimatedTaxPayment entities.
/// </summary>
public interface IEstimatedTaxPaymentRepository : IRepository<EstimatedTaxPayment>
{
    /// <summary>
    /// Gets all estimated tax payments for a specific tax year.
    /// </summary>
    Task<IReadOnlyList<EstimatedTaxPayment>> GetByTaxYearAsync(
        int taxYear,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the estimated tax payment for a specific quarter.
    /// </summary>
    Task<EstimatedTaxPayment?> GetByQuarterAsync(
        int taxYear,
        int quarter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all overdue (unpaid and past due date) estimated tax payments.
    /// </summary>
    Task<IReadOnlyList<EstimatedTaxPayment>> GetOverdueAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unpaid estimated tax payments.
    /// </summary>
    Task<IReadOnlyList<EstimatedTaxPayment>> GetUnpaidAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next upcoming payment deadline.
    /// </summary>
    Task<EstimatedTaxPayment?> GetNextUpcomingAsync(
        CancellationToken cancellationToken = default);
}
