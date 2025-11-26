using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get expenses by category.
/// </summary>
public record GetExpensesByCategoryQuery(ExpenseCategory Category);

/// <summary>
/// Handler for GetExpensesByCategoryQuery.
/// </summary>
public class GetExpensesByCategoryQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpensesByCategoryQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BusinessExpenseDto>> HandleAsync(
        GetExpensesByCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.GetByCategoryAsync(query.Category, cancellationToken);
        return expenses.Select(GetExpenseQueryHandler.MapToDto).ToList();
    }
}
