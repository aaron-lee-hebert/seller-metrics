using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Inventory.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the inventory list page.
/// </summary>
public class InventoryListViewModel
{
    public List<InventoryItemDto> Items { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public InventoryStatus? StatusFilter { get; set; }
    public int? LocationFilter { get; set; }
    public string? SearchTerm { get; set; }

    // Filter options
    public List<SelectListItem> StatusOptions { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();

    // Summary
    public InventoryValueDto Summary { get; set; } = new();
}

/// <summary>
/// ViewModel for creating/editing inventory items.
/// </summary>
public class InventoryFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "eBay SKU")]
    [StringLength(50)]
    public string? EbaySku { get; set; }

    [Required]
    [Display(Name = "Title")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Cost (COGS)")]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal CogsAmount { get; set; }

    [Display(Name = "Currency")]
    public string CogsCurrency { get; set; } = "USD";

    [Required]
    [Display(Name = "Quantity")]
    [Range(1, 10000)]
    public int Quantity { get; set; } = 1;

    [Display(Name = "Purchase Date")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDate { get; set; }

    [Display(Name = "Storage Location")]
    public int? StorageLocationId { get; set; }

    [Display(Name = "Status")]
    public InventoryStatus Status { get; set; } = InventoryStatus.Unlisted;

    [Display(Name = "Condition")]
    public EbayCondition? Condition { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Photo Path")]
    [StringLength(500)]
    public string? PhotoPath { get; set; }

    // Dropdown options
    public List<SelectListItem> StatusOptions { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
    public List<SelectListItem> ConditionOptions { get; set; } = new();

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Inventory Item" : "Add Inventory Item";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Item";
}

/// <summary>
/// ViewModel for inventory item details page.
/// </summary>
public class InventoryDetailViewModel
{
    public InventoryItemDto Item { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
}

/// <summary>
/// ViewModel for moving inventory to a new location.
/// </summary>
public class MoveInventoryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CurrentLocation { get; set; }

    [Display(Name = "New Location")]
    public int? NewStorageLocationId { get; set; }

    public List<SelectListItem> LocationOptions { get; set; } = new();
}
