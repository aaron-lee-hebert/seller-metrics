using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Wave.DTOs;

/// <summary>
/// DTO for displaying a Wave invoice in lists.
/// </summary>
public class WaveInvoiceListItemDto
{
    public int Id { get; set; }
    public string WaveInvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public WaveInvoiceStatus Status { get; set; }
    public bool IsOverdue { get; set; }
    public string? ViewUrl { get; set; }
}

/// <summary>
/// DTO for displaying detailed Wave invoice information.
/// </summary>
public class WaveInvoiceDetailDto
{
    public int Id { get; set; }
    public string WaveInvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? WaveCustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public WaveInvoiceStatus Status { get; set; }
    public string? Memo { get; set; }
    public string? ViewUrl { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime LastSyncedAt { get; set; }
    public List<WavePaymentDetailDto> Payments { get; set; } = new();
}

/// <summary>
/// DTO for displaying a Wave payment.
/// </summary>
public class WavePaymentDetailDto
{
    public int Id { get; set; }
    public string WavePaymentId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for Wave invoice statistics.
/// </summary>
public class WaveInvoiceStatsDto
{
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int UnpaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalOutstanding { get; set; }
    public string Currency { get; set; } = "USD";
}
