using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Commands;

/// <summary>
/// Command to delete a business expense (soft delete).
/// </summary>
public record DeleteExpenseCommand(int Id);

/// <summary>
/// Handler for DeleteExpenseCommand.
/// </summary>
public class DeleteExpenseCommandHandler
{
    private readonly IBusinessExpenseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseCommandHandler(
        IBusinessExpenseRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteExpenseCommand command,
        CancellationToken cancellationToken = default)
    {
        var expense = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (expense == null)
        {
            throw new InvalidOperationException($"Business expense with ID {command.Id} not found.");
        }

        // Soft delete
        expense.IsDeleted = true;
        expense.DeletedAt = DateTime.UtcNow;
        expense.UpdatedAt = DateTime.UtcNow;

        _repository.Update(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
