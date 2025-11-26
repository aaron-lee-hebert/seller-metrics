using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Commands;

/// <summary>
/// Command to delete a mileage entry (soft delete).
/// </summary>
public record DeleteMileageEntryCommand(int Id);

/// <summary>
/// Handler for DeleteMileageEntryCommand.
/// </summary>
public class DeleteMileageEntryCommandHandler
{
    private readonly IMileageEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMileageEntryCommandHandler(
        IMileageEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteMileageEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (entry == null)
        {
            throw new InvalidOperationException($"Mileage entry with ID {command.Id} not found.");
        }

        // Soft delete
        entry.IsDeleted = true;
        entry.DeletedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
