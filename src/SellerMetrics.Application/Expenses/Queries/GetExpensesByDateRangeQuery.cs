using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get expenses within a date range.
/// </summary>
public record GetExpensesByDateRangeQuery(DateTime StartDate, DateTime EndDate);

/// <summary>
/// Handler for GetExpensesByDateRangeQuery.
/// </summary>
public class GetExpensesByDateRangeQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpensesByDateRangeQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BusinessExpenseDto>> HandleAsync(
        GetExpensesByDateRangeQuery query,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        return expenses.Select(GetExpenseQueryHandler.MapToDto).ToList();
    }
}
