using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Revenue.Commands;

/// <summary>
/// Command to delete a revenue entry (soft delete).
/// </summary>
public record DeleteRevenueEntryCommand(int Id);

/// <summary>
/// Handler for DeleteRevenueEntryCommand.
/// </summary>
public class DeleteRevenueEntryCommandHandler
{
    private readonly IRevenueEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRevenueEntryCommandHandler(
        IRevenueEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteRevenueEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (entry == null)
        {
            throw new InvalidOperationException($"Revenue entry with ID {command.Id} not found.");
        }

        // Soft delete
        entry.IsDeleted = true;
        entry.DeletedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
