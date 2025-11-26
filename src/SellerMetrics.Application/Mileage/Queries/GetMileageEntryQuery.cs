using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Queries;

/// <summary>
/// Query to get a single mileage entry by ID.
/// </summary>
public record GetMileageEntryQuery(int Id);

/// <summary>
/// Handler for GetMileageEntryQuery.
/// </summary>
public class GetMileageEntryQueryHandler
{
    private readonly IMileageEntryRepository _repository;

    public GetMileageEntryQueryHandler(IMileageEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<MileageEntryDto?> HandleAsync(
        GetMileageEntryQuery query,
        CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (entry == null)
        {
            return null;
        }

        return GetMileageLogQueryHandler.MapToDto(entry);
    }
}
