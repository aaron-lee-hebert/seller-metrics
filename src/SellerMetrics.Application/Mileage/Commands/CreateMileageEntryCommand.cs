using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Commands;

/// <summary>
/// Command to create a new mileage entry.
/// </summary>
public record CreateMileageEntryCommand(
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
/// Handler for CreateMileageEntryCommand.
/// </summary>
public class CreateMileageEntryCommandHandler
{
    private readonly IMileageEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMileageEntryCommandHandler(
        IMileageEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(
        CreateMileageEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entry = new MileageEntry
        {
            TripDate = command.TripDate,
            Purpose = command.Purpose,
            StartLocation = command.StartLocation,
            Destination = command.Destination,
            Miles = command.Miles,
            IsRoundTrip = command.IsRoundTrip,
            BusinessLine = command.BusinessLine,
            Notes = command.Notes,
            ServiceJobId = command.ServiceJobId,
            OdometerStart = command.OdometerStart,
            OdometerEnd = command.OdometerEnd
        };

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
