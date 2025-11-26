namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Status of a service/repair job.
/// </summary>
public enum ServiceJobStatus
{
    /// <summary>
    /// Job has been created but work has not started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Work is in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Waiting for parts or customer response.
    /// </summary>
    OnHold = 2,

    /// <summary>
    /// Work is complete, awaiting customer pickup/delivery.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Job has been delivered and paid.
    /// </summary>
    Closed = 4,

    /// <summary>
    /// Job was cancelled.
    /// </summary>
    Cancelled = 5
}
