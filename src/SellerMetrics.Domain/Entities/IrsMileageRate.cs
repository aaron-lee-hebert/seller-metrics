using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Stores IRS mileage rates by year for deduction calculations.
/// Rates are stored per mile (e.g., 0.67 for $0.67/mile).
/// </summary>
public class IrsMileageRate : BaseEntity
{
    /// <summary>
    /// The calendar year this rate applies to.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Standard mileage rate per mile (e.g., 0.67 for 67 cents).
    /// This is the rate used for most business travel.
    /// </summary>
    public decimal StandardRate { get; set; }

    /// <summary>
    /// Medical/moving mileage rate per mile (if applicable).
    /// </summary>
    public decimal? MedicalRate { get; set; }

    /// <summary>
    /// Charitable mileage rate per mile (if applicable).
    /// </summary>
    public decimal? CharitableRate { get; set; }

    /// <summary>
    /// Effective start date for this rate (usually January 1).
    /// The IRS sometimes changes rates mid-year.
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Optional notes about this rate (e.g., "Mid-year increase due to gas prices").
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Calculates the deduction amount for a given number of miles.
    /// </summary>
    /// <param name="miles">Number of miles driven.</param>
    /// <returns>The deduction amount.</returns>
    public decimal CalculateDeduction(decimal miles)
    {
        return miles * StandardRate;
    }
}
