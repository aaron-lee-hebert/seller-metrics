using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Web.Models;

/// <summary>
/// View model for the eBay orders list page.
/// </summary>
public class EbayOrderListViewModel
{
    public IReadOnlyList<EbayOrderSummaryDto> Orders { get; set; } = new List<EbayOrderSummaryDto>();
    public EbayOrderStatsDto? Stats { get; set; }
    public EbayConnectionStatusDto? ConnectionStatus { get; set; }
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EbayOrderStatus? StatusFilter { get; set; }
    public bool? LinkedOnlyFilter { get; set; }

    // Filter options
    public List<SelectListItem> StatusOptions { get; set; } = new();
    public List<SelectListItem> LinkedOnlyOptions { get; set; } = new();
}

/// <summary>
/// View model for the eBay order details page.
/// </summary>
public class EbayOrderDetailViewModel
{
    public EbayOrderDisplayDto Order { get; set; } = new();
}

/// <summary>
/// View model for linking an order to inventory.
/// </summary>
public class LinkOrderToInventoryViewModel
{
    public int OrderId { get; set; }

    [Required]
    [Display(Name = "Inventory Item")]
    public int InventoryItemId { get; set; }
}

/// <summary>
/// View model for updating shipping cost.
/// </summary>
public class UpdateShippingCostViewModel
{
    public int OrderId { get; set; }

    [Required]
    [Range(0, 9999.99, ErrorMessage = "Shipping cost must be between $0 and $9,999.99")]
    [Display(Name = "Actual Shipping Cost")]
    public decimal ShippingAmount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// View model for eBay connection settings.
/// </summary>
public class EbayConnectionViewModel
{
    public EbayConnectionStatusDto Status { get; set; } = new();
    public string? AuthorizationUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
