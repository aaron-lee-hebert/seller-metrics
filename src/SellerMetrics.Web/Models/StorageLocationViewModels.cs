using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.StorageLocations.DTOs;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the storage locations list/tree view.
/// </summary>
public class StorageLocationListViewModel
{
    public List<StorageLocationDto> Locations { get; set; } = new();
    public int TotalLocations { get; set; }
    public int TotalRootLocations { get; set; }
}

/// <summary>
/// ViewModel for storage location details.
/// </summary>
public class StorageLocationDetailViewModel
{
    public StorageLocationDto Location { get; set; } = new();
    public List<StorageLocationDto> Children { get; set; } = new();
    public List<StorageLocationDto> Ancestors { get; set; } = new();

    // Items stored in this location
    public int InventoryItemCount { get; set; }
    public int ComponentCount { get; set; }
}

/// <summary>
/// ViewModel for creating/editing storage locations.
/// </summary>
public class StorageLocationFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Location Name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Parent Location")]
    public int? ParentId { get; set; }

    [Display(Name = "Sort Order")]
    [Range(0, 9999)]
    public int SortOrder { get; set; }

    // Dropdown options
    public List<SelectListItem> ParentOptions { get; set; } = new();

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Location" : "Add Location";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Location";
}
