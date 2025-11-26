using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a mileage log entry for business travel.
/// IRS-compliant with date, purpose, locations, and miles.
/// </summary>
public class MileageEntry : BaseEntity
{
    /// <summary>
    /// Date of the trip.
    /// </summary>
    public DateTime TripDate { get; set; }

    /// <summary>
    /// Business purpose of the trip (e.g., "Post office - ship orders", "Client visit - Smith residence").
    /// Required for IRS compliance.
    /// </summary>
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// Starting location of the trip.
    /// </summary>
    public string StartLocation { get; set; } = string.Empty;

    /// <summary>
    /// Destination of the trip.
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Miles driven (one-way distance).
    /// </summary>
    public decimal Miles { get; set; }

    /// <summary>
    /// Whether this was a round trip (doubles the miles for deduction).
    /// </summary>
    public bool IsRoundTrip { get; set; }

    /// <summary>
    /// Total deductible miles (Miles * 2 if round trip).
    /// </summary>
    public decimal TotalMiles => IsRoundTrip ? Miles * 2 : Miles;

    /// <summary>
    /// Business line this trip is associated with.
    /// </summary>
    public BusinessLine BusinessLine { get; set; }

    /// <summary>
    /// Additional notes about the trip.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Optional link to a service job (for service calls).
    /// </summary>
    public int? ServiceJobId { get; set; }

    /// <summary>
    /// Navigation property to the linked service job.
    /// </summary>
    public ServiceJob? ServiceJob { get; set; }

    /// <summary>
    /// Odometer reading at start (optional for detailed tracking).
    /// </summary>
    public int? OdometerStart { get; set; }

    /// <summary>
    /// Odometer reading at end (optional for detailed tracking).
    /// </summary>
    public int? OdometerEnd { get; set; }
}
