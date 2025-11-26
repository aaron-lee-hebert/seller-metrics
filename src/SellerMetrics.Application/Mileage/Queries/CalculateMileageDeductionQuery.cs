using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Mileage.Queries;

/// <summary>
/// Query to calculate mileage deduction for a date range.
/// </summary>
public record CalculateMileageDeductionQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Currency = "USD");

/// <summary>
/// Handler for CalculateMileageDeductionQuery.
/// </summary>
public class CalculateMileageDeductionQueryHandler
{
    private readonly IMileageEntryRepository _mileageRepository;
    private readonly IIrsMileageRateRepository _rateRepository;

    public CalculateMileageDeductionQueryHandler(
        IMileageEntryRepository mileageRepository,
        IIrsMileageRateRepository rateRepository)
    {
        _mileageRepository = mileageRepository;
        _rateRepository = rateRepository;
    }

    public async Task<MileageDeductionDto> HandleAsync(
        CalculateMileageDeductionQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get all mileage entries for the date range
        var entries = await _mileageRepository.GetByDateRangeAsync(
            query.StartDate,
            query.EndDate,
            cancellationToken);

        // Calculate totals by business line
        var ebayMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.eBay)
            .Sum(e => e.TotalMiles);

        var serviceMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.ComputerServices)
            .Sum(e => e.TotalMiles);

        var sharedMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.Shared)
            .Sum(e => e.TotalMiles);

        var totalMiles = ebayMiles + serviceMiles + sharedMiles;

        // Get the applicable IRS rate
        // Use the rate for the end date's year (or start of range if spans years)
        var rate = await _rateRepository.GetByDateAsync(query.EndDate, cancellationToken);
        var applicableRate = rate?.StandardRate ?? 0.67m; // Default to 2024 rate if not found
        var rateYear = rate?.Year ?? query.EndDate.Year;

        // Calculate deductions
        var ebayDeduction = ebayMiles * applicableRate;
        var serviceDeduction = serviceMiles * applicableRate;
        var sharedDeduction = sharedMiles * applicableRate;
        var totalDeduction = totalMiles * applicableRate;

        return new MileageDeductionDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TotalMiles = totalMiles,
            EbayMiles = ebayMiles,
            ServiceMiles = serviceMiles,
            SharedMiles = sharedMiles,
            ApplicableRate = applicableRate,
            ApplicableRateFormatted = $"${applicableRate:N2}/mile",
            TotalDeduction = totalDeduction,
            EbayDeduction = ebayDeduction,
            ServiceDeduction = serviceDeduction,
            SharedDeduction = sharedDeduction,
            Currency = query.Currency,
            TotalDeductionFormatted = new Money(totalDeduction, query.Currency).ToString(),
            EbayDeductionFormatted = new Money(ebayDeduction, query.Currency).ToString(),
            ServiceDeductionFormatted = new Money(serviceDeduction, query.Currency).ToString(),
            SharedDeductionFormatted = new Money(sharedDeduction, query.Currency).ToString(),
            TripCount = entries.Count,
            RateYear = rateYear
        };
    }
}
