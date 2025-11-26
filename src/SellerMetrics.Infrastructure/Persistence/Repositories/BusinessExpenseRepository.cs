using Microsoft.EntityFrameworkCore;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for business expense operations.
/// </summary>
public class BusinessExpenseRepository : RepositoryBase<BusinessExpense>, IBusinessExpenseRepository
{
    public BusinessExpenseRepository(SellerMetricsDbContext context) : base(context)
    {
    }

    public override async Task<BusinessExpense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.ServiceJob)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<BusinessExpense>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.ServiceJob)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessExpense>> GetByCategoryAsync(
        ExpenseCategory category,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.ServiceJob)
            .Where(e => e.Category == category)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessExpense>> GetByBusinessLineAsync(
        BusinessLine businessLine,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.ServiceJob)
            .Where(e => e.BusinessLine == businessLine)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessExpense>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.ServiceJob)
            .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessExpense>> GetByServiceJobIdAsync(
        int serviceJobId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.ServiceJob)
            .Where(e => e.ServiceJobId == serviceJobId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(ExpenseCategory Category, decimal Total)>> GetTotalsByCategoryAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        var results = await _dbSet
            .Where(e => e.ExpenseDate >= startDate &&
                        e.ExpenseDate <= endDate &&
                        e.Amount.Currency == currency &&
                        e.IsTaxDeductible)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount.Amount) })
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Category, r.Total)).ToList();
    }

    public async Task<IReadOnlyList<(BusinessLine BusinessLine, decimal Total)>> GetTotalsByBusinessLineAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        var results = await _dbSet
            .Where(e => e.ExpenseDate >= startDate &&
                        e.ExpenseDate <= endDate &&
                        e.Amount.Currency == currency &&
                        e.IsTaxDeductible)
            .GroupBy(e => e.BusinessLine)
            .Select(g => new { BusinessLine = g.Key, Total = g.Sum(e => e.Amount.Amount) })
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.BusinessLine, r.Total)).ToList();
    }

    public async Task<decimal> GetTotalExpensesAsync(
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.ExpenseDate >= startDate &&
                        e.ExpenseDate <= endDate &&
                        e.Amount.Currency == currency &&
                        e.IsTaxDeductible)
            .SumAsync(e => e.Amount.Amount, cancellationToken);
    }
}
