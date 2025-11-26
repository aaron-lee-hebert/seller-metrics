using SellerMetrics.Domain.Common;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Represents a computer repair/service job.
/// Components can be reserved or used for a specific service job.
/// </summary>
public class ServiceJob : BaseEntity
{
    /// <summary>
    /// Job reference number for tracking.
    /// Format: SVC-YYYYMMDD-XXXX
    /// </summary>
    public string JobNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name for this job.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer contact information (phone/email).
    /// </summary>
    public string? CustomerContact { get; set; }

    /// <summary>
    /// Description of the work to be performed.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the job.
    /// </summary>
    public ServiceJobStatus Status { get; set; } = ServiceJobStatus.Pending;

    /// <summary>
    /// Date the job was received/started.
    /// </summary>
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date the job was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Additional notes about the job.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Wave invoice ID if this job has been invoiced.
    /// </summary>
    public string? WaveInvoiceId { get; set; }

    /// <summary>
    /// Components reserved or used for this job.
    /// </summary>
    public ICollection<ComponentItem> Components { get; set; } = new List<ComponentItem>();

    /// <summary>
    /// Generates a unique job number.
    /// Format: SVC-YYYYMMDD-XXXX (e.g., SVC-20250601-0001)
    /// </summary>
    public static string GenerateJobNumber(int sequence)
    {
        return $"SVC-{DateTime.UtcNow:yyyyMMdd}-{sequence:D4}";
    }
}
