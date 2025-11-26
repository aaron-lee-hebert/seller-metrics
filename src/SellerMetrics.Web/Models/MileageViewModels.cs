using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the mileage log list page.
/// </summary>
public class MileageListViewModel
{
    public List<MileageEntryDto> Entries { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BusinessLine? BusinessLineFilter { get; set; }

    // Summary & Deduction
    public MileageDeductionDto Deduction { get; set; } = new();
    public IrsMileageRateDto? CurrentRate { get; set; }

    // Filter options
    public List<SelectListItem> BusinessLineOptions { get; set; } = new();
}

/// <summary>
/// ViewModel for mileage entry details.
/// </summary>
public class MileageDetailViewModel
{
    public MileageEntryDto Entry { get; set; } = new();
    public decimal EstimatedDeduction { get; set; }
    public string EstimatedDeductionFormatted { get; set; } = "$0.00";
    public IrsMileageRateDto? ApplicableRate { get; set; }
}

/// <summary>
/// ViewModel for creating/editing mileage entries.
/// </summary>
public class MileageFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Trip Date")]
    [DataType(DataType.Date)]
    public DateTime TripDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Business Purpose")]
    [StringLength(500)]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Starting Location")]
    [StringLength(200)]
    public string StartLocation { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Destination")]
    [StringLength(200)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [Display(Name = "One-Way Miles")]
    [Range(0.1, 9999.9)]
    public decimal Miles { get; set; }

    [Display(Name = "Round Trip")]
    public bool IsRoundTrip { get; set; }

    [Required]
    [Display(Name = "Business Line")]
    public BusinessLine BusinessLine { get; set; } = BusinessLine.eBay;

    [Display(Name = "Odometer Start")]
    [Range(0, 999999)]
    public int? OdometerStart { get; set; }

    [Display(Name = "Odometer End")]
    [Range(0, 999999)]
    public int? OdometerEnd { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // For linking to service jobs
    public int? ServiceJobId { get; set; }

    // Dropdown options
    public List<SelectListItem> BusinessLineOptions { get; set; } = new();

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Mileage Entry" : "Log Trip";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Log Trip";

    // Calculated display
    public decimal TotalMiles => IsRoundTrip ? Miles * 2 : Miles;
}
