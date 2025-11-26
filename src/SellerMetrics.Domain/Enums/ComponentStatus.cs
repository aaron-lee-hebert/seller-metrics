namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Status of a component item in the system.
/// </summary>
public enum ComponentStatus
{
    /// <summary>
    /// Component is available for use or sale.
    /// </summary>
    Available = 0,

    /// <summary>
    /// Component is reserved for a specific service job.
    /// </summary>
    Reserved = 1,

    /// <summary>
    /// Component has been used in a repair/service job.
    /// </summary>
    Used = 2,

    /// <summary>
    /// Component has been sold separately (not as part of a service job).
    /// </summary>
    Sold = 3
}
