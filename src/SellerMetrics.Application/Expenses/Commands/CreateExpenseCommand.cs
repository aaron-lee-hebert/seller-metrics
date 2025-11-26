using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Expenses.Commands;

/// <summary>
/// Command to create a new business expense.
/// </summary>
public record CreateExpenseCommand(
    DateTime ExpenseDate,
    string Description,
    decimal Amount,
    ExpenseCategory Category,
    BusinessLine BusinessLine,
    string Currency = "USD",
    string? Vendor = null,
    string? ReceiptPath = null,
    string? Notes = null,
    int? ServiceJobId = null,
    bool IsTaxDeductible = true,
    string? ReferenceNumber = null);

/// <summary>
/// Handler for CreateExpenseCommand.
/// </summary>
public class CreateExpenseCommandHandler
{
    private readonly IBusinessExpenseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IBusinessExpenseRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(
        CreateExpenseCommand command,
        CancellationToken cancellationToken = default)
    {
        var expense = new BusinessExpense
        {
            ExpenseDate = command.ExpenseDate,
            Description = command.Description,
            Amount = new Money(command.Amount, command.Currency),
            Category = command.Category,
            BusinessLine = command.BusinessLine,
            Vendor = command.Vendor,
            ReceiptPath = command.ReceiptPath,
            Notes = command.Notes,
            ServiceJobId = command.ServiceJobId,
            IsTaxDeductible = command.IsTaxDeductible,
            ReferenceNumber = command.ReferenceNumber
        };

        await _repository.AddAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }
}
