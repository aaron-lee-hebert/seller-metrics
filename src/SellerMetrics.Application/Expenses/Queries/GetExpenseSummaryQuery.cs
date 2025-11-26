using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get expense summary for a date range.
/// </summary>
public record GetExpenseSummaryQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for GetExpenseSummaryQuery.
/// </summary>
public class GetExpenseSummaryQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpenseSummaryQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpenseSummaryDto> HandleAsync(
        GetExpenseSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        var filteredExpenses = expenses
            .Where(e => e.Amount.Currency == query.Currency && e.IsTaxDeductible)
            .ToList();

        var totalExpenses = filteredExpenses.Sum(e => e.Amount.Amount);

        // Group by category
        var byCategory = filteredExpenses
            .GroupBy(e => e.Category)
            .Select(g => new ExpenseByCategoryDto
            {
                Category = g.Key,
                CategoryDisplay = g.Key.GetDisplayName(),
                ScheduleCLine = g.Key.GetScheduleCLine(),
                ScheduleCLineLabel = g.Key.GetScheduleCLineLabel(),
                ScheduleCDescription = g.Key.GetScheduleCDescription(),
                Total = g.Sum(e => e.Amount.Amount),
                Currency = query.Currency,
                TotalFormatted = new Money(g.Sum(e => e.Amount.Amount), query.Currency).ToString(),
                ExpenseCount = g.Count()
            })
            .OrderBy(c => c.ScheduleCLine) // Order by Schedule C line number
            .ToList();

        // Group by business line
        var byBusinessLine = filteredExpenses
            .GroupBy(e => e.BusinessLine)
            .Select(g => new ExpenseByBusinessLineDto
            {
                BusinessLine = g.Key,
                BusinessLineDisplay = g.Key == BusinessLine.eBay ? "eBay" : g.Key.ToString(),
                Total = g.Sum(e => e.Amount.Amount),
                Currency = query.Currency,
                TotalFormatted = new Money(g.Sum(e => e.Amount.Amount), query.Currency).ToString(),
                ExpenseCount = g.Count()
            })
            .OrderByDescending(b => b.Total)
            .ToList();

        return new ExpenseSummaryDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TotalExpenses = totalExpenses,
            Currency = query.Currency,
            TotalExpensesFormatted = new Money(totalExpenses, query.Currency).ToString(),
            TotalCount = filteredExpenses.Count,
            ByCategory = byCategory,
            ByBusinessLine = byBusinessLine
        };
    }
}
