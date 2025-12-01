using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Application.Wave.DTOs;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the service invoices list page.
/// </summary>
public class InvoiceListViewModel
{
    // Manual revenue entries (when Wave not connected)
    public List<RevenueEntryDto> Invoices { get; set; } = new();

    // Wave invoices (when connected)
    public List<WaveInvoiceListItemDto> WaveInvoices { get; set; } = new();

    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Summary for manual entries
    public InvoiceSummaryViewModel Summary { get; set; } = new();

    // Summary for Wave invoices
    public WaveInvoiceSummaryViewModel WaveSummary { get; set; } = new();

    // Sync status
    public bool IsWaveSyncConfigured { get; set; }
    public DateTime? LastSyncTime { get; set; }
}

/// <summary>
/// Summary statistics for manual service invoices.
/// </summary>
public class InvoiceSummaryViewModel
{
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TotalRevenueFormatted { get; set; } = "$0.00";
}

/// <summary>
/// Summary statistics for Wave invoices.
/// </summary>
public class WaveInvoiceSummaryViewModel
{
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int UnpaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TotalRevenueFormatted { get; set; } = "$0.00";
    public decimal TotalOutstanding { get; set; }
    public string TotalOutstandingFormatted { get; set; } = "$0.00";
}

/// <summary>
/// ViewModel for service invoice details.
/// </summary>
public class InvoiceDetailViewModel
{
    public RevenueEntryDto Invoice { get; set; } = new();
}

/// <summary>
/// ViewModel for manually adding a service invoice.
/// </summary>
public class ManualInvoiceFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Invoice Number")]
    [StringLength(50)]
    public string WaveInvoiceNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Invoice Date")]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Customer/Description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Invoice Amount")]
    [Range(0.01, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal GrossAmount { get; set; }

    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Invoice" : "Add Manual Invoice";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Invoice";
}
