using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get expenses by business line.
/// </summary>
public record GetExpensesByBusinessLineQuery(BusinessLine BusinessLine);

/// <summary>
/// Handler for GetExpensesByBusinessLineQuery.
/// </summary>
public class GetExpensesByBusinessLineQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpensesByBusinessLineQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BusinessExpenseDto>> HandleAsync(
        GetExpensesByBusinessLineQuery query,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.GetByBusinessLineAsync(query.BusinessLine, cancellationToken);
        return expenses.Select(GetExpenseQueryHandler.MapToDto).ToList();
    }
}
