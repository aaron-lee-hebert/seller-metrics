using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for business expense operations.
/// </summary>
public interface IBusinessExpenseRepository : IRepository<BusinessExpense>
{
    /// <summary>
    /// Gets expenses by category.
    /// </summary>
    Task<IReadOnlyList<BusinessExpense>> GetByCategoryAsync(
        ExpenseCategory category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expenses by business line.
    /// </summary>
    Task<IReadOnlyList<BusinessExpense>> GetByBusinessLineAsync(
        BusinessLine businessLine,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expenses within a date range.
    /// </summary>
    Task<IReadOnlyList<BusinessExpense>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expenses for a specific service job.
    /// </summary>
    Task<IReadOnlyList<BusinessExpense>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total expenses by category for a date range.
    /// </summary>
    Task<IReadOnlyList<(ExpenseCategory Category, decimal Total)>> GetTotalsByCategoryAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total expenses by business line for a date range.
    /// </summary>
    Task<IReadOnlyList<(BusinessLine BusinessLine, decimal Total)>> GetTotalsByBusinessLineAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total expenses for a date range.
    /// </summary>
    Task<decimal> GetTotalExpensesAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);
}
