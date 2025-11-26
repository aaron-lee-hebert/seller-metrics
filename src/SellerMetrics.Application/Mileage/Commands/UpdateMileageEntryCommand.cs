using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Commands;

/// <summary>
/// Command to update an existing mileage entry.
/// </summary>
public record UpdateMileageEntryCommand(
    int Id,
    DateTime TripDate,
    string Purpose,
    string StartLocation,
    string Destination,
    decimal Miles,
    BusinessLine BusinessLine,
    bool IsRoundTrip = false,
    string? Notes = null,
    int? ServiceJobId = null,
    int? OdometerStart = null,
    int? OdometerEnd = null);

/// <summary>
/// Handler for UpdateMileageEntryCommand.
/// </summary>
public class UpdateMileageEntryCommandHandler
{
    private readonly IMileageEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMileageEntryCommandHandler(
        IMileageEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateMileageEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (entry == null)
        {
            throw new InvalidOperationException($"Mileage entry with ID {command.Id} not found.");
        }

        entry.TripDate = command.TripDate;
        entry.Purpose = command.Purpose;
        entry.StartLocation = command.StartLocation;
        entry.Destination = command.Destination;
        entry.Miles = command.Miles;
        entry.IsRoundTrip = command.IsRoundTrip;
        entry.BusinessLine = command.BusinessLine;
        entry.Notes = command.Notes;
        entry.ServiceJobId = command.ServiceJobId;
        entry.OdometerStart = command.OdometerStart;
        entry.OdometerEnd = command.OdometerEnd;
        entry.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
