using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Configuration for fiscal year settings.
/// Allows the business to define fiscal year start month.
/// </summary>
public class FiscalYearConfiguration : BaseEntity
{
    /// <summary>
    /// The starting month of the fiscal year (1-12).
    /// Default is January (1) for calendar year alignment.
    /// </summary>
    public int FiscalYearStartMonth { get; set; } = 1;

    /// <summary>
    /// Whether this is the active configuration.
    /// Only one configuration should be active at a time.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the fiscal year for a given date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The fiscal year number.</returns>
    public int GetFiscalYear(DateTime date)
    {
        if (date.Month >= FiscalYearStartMonth)
        {
            return date.Year;
        }
        return date.Year - 1;
    }

    /// <summary>
    /// Gets the fiscal quarter for a given date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The fiscal quarter (1-4).</returns>
    public int GetFiscalQuarter(DateTime date)
    {
        int monthsFromStart = (date.Month - FiscalYearStartMonth + 12) % 12;
        return (monthsFromStart / 3) + 1;
    }

    /// <summary>
    /// Gets the start date of a fiscal year.
    /// </summary>
    /// <param name="fiscalYear">The fiscal year.</param>
    /// <returns>The start date of the fiscal year.</returns>
    public DateTime GetFiscalYearStart(int fiscalYear)
    {
        return new DateTime(fiscalYear, FiscalYearStartMonth, 1);
    }

    /// <summary>
    /// Gets the end date of a fiscal year.
    /// </summary>
    /// <param name="fiscalYear">The fiscal year.</param>
    /// <returns>The end date of the fiscal year.</returns>
    public DateTime GetFiscalYearEnd(int fiscalYear)
    {
        return GetFiscalYearStart(fiscalYear + 1).AddDays(-1);
    }

    /// <summary>
    /// Gets the start date of a fiscal quarter.
    /// </summary>
    /// <param name="fiscalYear">The fiscal year.</param>
    /// <param name="quarter">The quarter (1-4).</param>
    /// <returns>The start date of the quarter.</returns>
    public DateTime GetQuarterStart(int fiscalYear, int quarter)
    {
        if (quarter < 1 || quarter > 4)
            throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be between 1 and 4.");

        int monthsFromStart = (quarter - 1) * 3;
        int startMonth = ((FiscalYearStartMonth - 1 + monthsFromStart) % 12) + 1;
        int year = fiscalYear;

        // Adjust year if the quarter wraps into the next calendar year
        if (startMonth < FiscalYearStartMonth)
        {
            year++;
        }

        return new DateTime(year, startMonth, 1);
    }

    /// <summary>
    /// Gets the end date of a fiscal quarter.
    /// </summary>
    /// <param name="fiscalYear">The fiscal year.</param>
    /// <param name="quarter">The quarter (1-4).</param>
    /// <returns>The end date of the quarter.</returns>
    public DateTime GetQuarterEnd(int fiscalYear, int quarter)
    {
        if (quarter == 4)
        {
            return GetFiscalYearEnd(fiscalYear);
        }
        return GetQuarterStart(fiscalYear, quarter + 1).AddDays(-1);
    }
}
