using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Mileage.Queries;

/// <summary>
/// Query to get all IRS mileage rates.
/// </summary>
public record GetIrsMileageRatesQuery;

/// <summary>
/// Handler for GetIrsMileageRatesQuery.
/// </summary>
public class GetIrsMileageRatesQueryHandler
{
    private readonly IIrsMileageRateRepository _repository;

    public GetIrsMileageRatesQueryHandler(IIrsMileageRateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<IrsMileageRateDto>> HandleAsync(
        GetIrsMileageRatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var rates = await _repository.GetAllOrderedAsync(cancellationToken);

        return rates.Select(r => new IrsMileageRateDto
        {
            Id = r.Id,
            Year = r.Year,
            StandardRate = r.StandardRate,
            StandardRateFormatted = $"${r.StandardRate:N2}/mile",
            MedicalRate = r.MedicalRate,
            CharitableRate = r.CharitableRate,
            EffectiveDate = r.EffectiveDate,
            Notes = r.Notes
        }).ToList();
    }
}
