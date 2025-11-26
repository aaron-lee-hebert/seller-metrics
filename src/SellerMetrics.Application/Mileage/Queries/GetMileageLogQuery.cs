using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Queries;

/// <summary>
/// Query to get mileage log entries with optional filters.
/// </summary>
public record GetMileageLogQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    BusinessLine? BusinessLine = null);

/// <summary>
/// Handler for GetMileageLogQuery.
/// </summary>
public class GetMileageLogQueryHandler
{
    private readonly IMileageEntryRepository _repository;

    public GetMileageLogQueryHandler(IMileageEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MileageEntryDto>> HandleAsync(
        GetMileageLogQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MileageEntry> entries;

        if (query.BusinessLine.HasValue && query.StartDate.HasValue && query.EndDate.HasValue)
        {
            entries = await _repository.GetByBusinessLineAndDateRangeAsync(
                query.BusinessLine.Value,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            entries = await _repository.GetByDateRangeAsync(
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else if (query.BusinessLine.HasValue)
        {
            entries = await _repository.GetByBusinessLineAsync(
                query.BusinessLine.Value,
                cancellationToken);
        }
        else
        {
            entries = await _repository.GetAllAsync(cancellationToken);
        }

        return entries.Select(MapToDto).ToList();
    }

    internal static MileageEntryDto MapToDto(MileageEntry entry)
    {
        return new MileageEntryDto
        {
            Id = entry.Id,
            TripDate = entry.TripDate,
            Purpose = entry.Purpose,
            StartLocation = entry.StartLocation,
            Destination = entry.Destination,
            Miles = entry.Miles,
            IsRoundTrip = entry.IsRoundTrip,
            TotalMiles = entry.TotalMiles,
            BusinessLine = entry.BusinessLine,
            BusinessLineDisplay = entry.BusinessLine == BusinessLine.eBay
                ? "eBay"
                : entry.BusinessLine.ToString(),
            Notes = entry.Notes,
            ServiceJobId = entry.ServiceJobId,
            ServiceJobNumber = entry.ServiceJob?.JobNumber,
            OdometerStart = entry.OdometerStart,
            OdometerEnd = entry.OdometerEnd,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
    }
}
