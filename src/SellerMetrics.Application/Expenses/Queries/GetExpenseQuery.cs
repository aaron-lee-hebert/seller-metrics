using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get a single expense by ID.
/// </summary>
public record GetExpenseQuery(int Id);

/// <summary>
/// Handler for GetExpenseQuery.
/// </summary>
public class GetExpenseQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpenseQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<BusinessExpenseDto?> HandleAsync(
        GetExpenseQuery query,
        CancellationToken cancellationToken = default)
    {
        var expense = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (expense == null)
        {
            return null;
        }

        return MapToDto(expense);
    }

    internal static BusinessExpenseDto MapToDto(BusinessExpense expense)
    {
        return new BusinessExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            Description = expense.Description,
            Amount = expense.Amount.Amount,
            Currency = expense.Amount.Currency,
            AmountFormatted = expense.Amount.ToString(),
            Category = expense.Category,
            CategoryDisplay = expense.Category.GetDisplayName(),
            ScheduleCLine = expense.Category.GetScheduleCLine(),
            ScheduleCLineLabel = expense.Category.GetScheduleCLineLabel(),
            BusinessLine = expense.BusinessLine,
            BusinessLineDisplay = expense.BusinessLine == BusinessLine.eBay
                ? "eBay"
                : expense.BusinessLine.ToString(),
            Vendor = expense.Vendor,
            ReceiptPath = expense.ReceiptPath,
            Notes = expense.Notes,
            ServiceJobId = expense.ServiceJobId,
            ServiceJobNumber = expense.ServiceJob?.JobNumber,
            IsTaxDeductible = expense.IsTaxDeductible,
            ReferenceNumber = expense.ReferenceNumber,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
