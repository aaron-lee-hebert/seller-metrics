using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Revenue.DTOs;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the service invoices list page.
/// </summary>
public class InvoiceListViewModel
{
    public List<RevenueEntryDto> Invoices { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Summary
    public InvoiceSummaryViewModel Summary { get; set; } = new();

    // Sync status
    public bool IsWaveSyncConfigured { get; set; }
    public DateTime? LastSyncTime { get; set; }
}

/// <summary>
/// Summary statistics for service invoices.
/// </summary>
public class InvoiceSummaryViewModel
{
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TotalRevenueFormatted { get; set; } = "$0.00";
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

    [Required]
    [Display(Name = "Sales Tax Collected")]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal TaxesCollectedAmount { get; set; }

    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Calculated properties
    public decimal InvoiceSubtotal => GrossAmount - TaxesCollectedAmount;
    public decimal TaxLiability => TaxesCollectedAmount > 0 ? 0 : InvoiceSubtotal * 0.0825m;
    public bool TaxesWereCollected => TaxesCollectedAmount > 0;

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Invoice" : "Add Manual Invoice";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Invoice";
}
