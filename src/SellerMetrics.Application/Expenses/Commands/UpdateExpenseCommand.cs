using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Expenses.Commands;

/// <summary>
/// Command to update an existing business expense.
/// </summary>
public record UpdateExpenseCommand(
    int Id,
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
/// Handler for UpdateExpenseCommand.
/// </summary>
public class UpdateExpenseCommandHandler
{
    private readonly IBusinessExpenseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseCommandHandler(
        IBusinessExpenseRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateExpenseCommand command,
        CancellationToken cancellationToken = default)
    {
        var expense = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (expense == null)
        {
            throw new InvalidOperationException($"Business expense with ID {command.Id} not found.");
        }

        expense.ExpenseDate = command.ExpenseDate;
        expense.Description = command.Description;
        expense.Amount = new Money(command.Amount, command.Currency);
        expense.Category = command.Category;
        expense.BusinessLine = command.BusinessLine;
        expense.Vendor = command.Vendor;
        expense.ReceiptPath = command.ReceiptPath;
        expense.Notes = command.Notes;
        expense.ServiceJobId = command.ServiceJobId;
        expense.IsTaxDeductible = command.IsTaxDeductible;
        expense.ReferenceNumber = command.ReferenceNumber;
        expense.UpdatedAt = DateTime.UtcNow;

        _repository.Update(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
