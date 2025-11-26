using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Mileage.DTOs;

/// <summary>
/// Data transfer object for MileageEntry.
/// </summary>
public class MileageEntryDto
{
    public int Id { get; set; }
    public DateTime TripDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string StartLocation { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal Miles { get; set; }
    public bool IsRoundTrip { get; set; }
    public decimal TotalMiles { get; set; }
    public BusinessLine BusinessLine { get; set; }
    public string BusinessLineDisplay { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? ServiceJobId { get; set; }
    public string? ServiceJobNumber { get; set; }
    public int? OdometerStart { get; set; }
    public int? OdometerEnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for IRS mileage rate.
/// </summary>
public class IrsMileageRateDto
{
    public int Id { get; set; }
    public int Year { get; set; }
    public decimal StandardRate { get; set; }
    public string StandardRateFormatted { get; set; } = string.Empty;
    public decimal? MedicalRate { get; set; }
    public decimal? CharitableRate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for mileage deduction calculation.
/// </summary>
public class MileageDeductionDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalMiles { get; set; }
    public decimal EbayMiles { get; set; }
    public decimal ServiceMiles { get; set; }
    public decimal SharedMiles { get; set; }
    public decimal ApplicableRate { get; set; }
    public string ApplicableRateFormatted { get; set; } = string.Empty;
    public decimal TotalDeduction { get; set; }
    public decimal EbayDeduction { get; set; }
    public decimal ServiceDeduction { get; set; }
    public decimal SharedDeduction { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalDeductionFormatted { get; set; } = string.Empty;
    public string EbayDeductionFormatted { get; set; } = string.Empty;
    public string ServiceDeductionFormatted { get; set; } = string.Empty;
    public string SharedDeductionFormatted { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int RateYear { get; set; }
}

/// <summary>
/// DTO for mileage log summary.
/// </summary>
public class MileageLogSummaryDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalMiles { get; set; }
    public int TripCount { get; set; }
    public IReadOnlyList<MileageByBusinessLineDto> ByBusinessLine { get; set; } = new List<MileageByBusinessLineDto>();
    public IReadOnlyList<MileageEntryDto> Entries { get; set; } = new List<MileageEntryDto>();
}

/// <summary>
/// DTO for mileage totals by business line.
/// </summary>
public class MileageByBusinessLineDto
{
    public BusinessLine BusinessLine { get; set; }
    public string BusinessLineDisplay { get; set; } = string.Empty;
    public decimal TotalMiles { get; set; }
    public int TripCount { get; set; }
}
