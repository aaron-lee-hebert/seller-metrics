using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the component list page.
/// </summary>
public class ComponentListViewModel
{
    public List<ComponentItemDto> Items { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public int? TypeFilter { get; set; }
    public int? LocationFilter { get; set; }
    public ComponentStatus? StatusFilter { get; set; }
    public bool ShowLowStockOnly { get; set; }

    // Filter options
    public List<SelectListItem> TypeOptions { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
    public List<SelectListItem> StatusOptions { get; set; } = new();

    // Summary
    public ComponentValueDto Summary { get; set; } = new();
}

/// <summary>
/// ViewModel for creating/editing component items.
/// </summary>
public class ComponentFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Component Type")]
    public int ComponentTypeId { get; set; }

    [Required]
    [Display(Name = "Description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Quantity")]
    [Range(1, 99999)]
    public int Quantity { get; set; } = 1;

    [Required]
    [Display(Name = "Unit Cost")]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal UnitCostAmount { get; set; }

    [Display(Name = "Currency")]
    public string UnitCostCurrency { get; set; } = "USD";

    [Display(Name = "Storage Location")]
    public int? StorageLocationId { get; set; }

    [Display(Name = "Acquired Date")]
    [DataType(DataType.Date)]
    public DateTime? AcquiredDate { get; set; }

    [Display(Name = "Source")]
    public ComponentSource Source { get; set; } = ComponentSource.Purchased;

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Dropdown options
    public List<SelectListItem> TypeOptions { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
    public List<SelectListItem> SourceOptions { get; set; } = new();

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Component" : "Add Component";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Component";
}

/// <summary>
/// ViewModel for component item details page.
/// </summary>
public class ComponentDetailViewModel
{
    public ComponentItemDto Item { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
    public List<ComponentQuantityAdjustmentDto> Adjustments { get; set; } = new();
}

/// <summary>
/// ViewModel for quantity adjustment.
/// </summary>
public class AdjustQuantityViewModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }

    [Required]
    [Display(Name = "Adjustment")]
    [Range(-9999, 9999)]
    public int Adjustment { get; set; }

    [Required]
    [Display(Name = "Reason")]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public int NewQuantity => CurrentQuantity + Adjustment;
}
