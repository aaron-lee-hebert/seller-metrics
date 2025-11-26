using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Application.Revenue.DTOs;

/// <summary>
/// Data transfer object for RevenueEntry.
/// </summary>
public class RevenueEntryDto
{
    public int Id { get; set; }
    public RevenueSource Source { get; set; }
    public string SourceDisplay { get; set; } = string.Empty;
    public RevenueEntryType EntryType { get; set; }
    public string EntryTypeDisplay { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal GrossAmount { get; set; }
    public decimal FeesAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string GrossFormatted { get; set; } = string.Empty;
    public string FeesFormatted { get; set; } = string.Empty;
    public string NetFormatted { get; set; } = string.Empty;
    public string? EbayOrderId { get; set; }
    public string? WaveInvoiceNumber { get; set; }
    public int? InventoryItemId { get; set; }
    public string? InventoryItemSku { get; set; }
    public int? ServiceJobId { get; set; }
    public string? ServiceJobNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for revenue summary by source.
/// </summary>
public class RevenueBySourceDto
{
    public RevenueSource Source { get; set; }
    public string SourceDisplay { get; set; } = string.Empty;
    public decimal GrossTotal { get; set; }
    public decimal FeesTotal { get; set; }
    public decimal NetTotal { get; set; }
    public string Currency { get; set; } = "USD";
    public string GrossFormatted { get; set; } = string.Empty;
    public string FeesFormatted { get; set; } = string.Empty;
    public string NetFormatted { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO for monthly revenue summary.
/// </summary>
public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal EbayRevenue { get; set; }
    public decimal ServiceRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Currency { get; set; } = "USD";
    public string EbayRevenueFormatted { get; set; } = string.Empty;
    public string ServiceRevenueFormatted { get; set; } = string.Empty;
    public string TotalRevenueFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for quarterly revenue summary.
/// </summary>
public class QuarterlyRevenueDto
{
    public int FiscalYear { get; set; }
    public int Quarter { get; set; }
    public string QuarterDisplay { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EbayRevenue { get; set; }
    public decimal ServiceRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Currency { get; set; } = "USD";
    public string EbayRevenueFormatted { get; set; } = string.Empty;
    public string ServiceRevenueFormatted { get; set; } = string.Empty;
    public string TotalRevenueFormatted { get; set; } = string.Empty;
}

/// <summary>
/// DTO for year-to-date revenue summary.
/// </summary>
public class YearToDateRevenueDto
{
    public int FiscalYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EbayRevenue { get; set; }
    public decimal ServiceRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Currency { get; set; } = "USD";
    public string EbayRevenueFormatted { get; set; } = string.Empty;
    public string ServiceRevenueFormatted { get; set; } = string.Empty;
    public string TotalRevenueFormatted { get; set; } = string.Empty;
    public IReadOnlyList<MonthlyRevenueDto> MonthlyBreakdown { get; set; } = new List<MonthlyRevenueDto>();
}
