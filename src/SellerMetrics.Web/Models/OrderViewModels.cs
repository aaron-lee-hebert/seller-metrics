using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Revenue.DTOs;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the eBay orders list page.
/// </summary>
public class OrderListViewModel
{
    public List<RevenueEntryDto> Orders { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Summary
    public OrderSummaryViewModel Summary { get; set; } = new();

    // Sync status
    public bool IsEbaySyncConfigured { get; set; }
    public DateTime? LastSyncTime { get; set; }
}

/// <summary>
/// Summary statistics for eBay orders.
/// </summary>
public class OrderSummaryViewModel
{
    public int TotalOrders { get; set; }
    public decimal TotalGross { get; set; }
    public string TotalGrossFormatted { get; set; } = "$0.00";
    public decimal TotalFees { get; set; }
    public string TotalFeesFormatted { get; set; } = "$0.00";
    public decimal TotalShipping { get; set; }
    public string TotalShippingFormatted { get; set; } = "$0.00";
    public decimal TotalNet { get; set; }
    public string TotalNetFormatted { get; set; } = "$0.00";
}

/// <summary>
/// ViewModel for eBay order details.
/// </summary>
public class OrderDetailViewModel
{
    public RevenueEntryDto Order { get; set; } = new();

    // Profit calculation (if linked to inventory)
    public bool HasCostData { get; set; }
    public decimal CostOfGoods { get; set; }
    public string CostOfGoodsFormatted { get; set; } = "$0.00";
    public decimal Profit { get; set; }
    public string ProfitFormatted { get; set; } = "$0.00";
    public decimal ProfitMargin { get; set; }
    public string ProfitMarginFormatted { get; set; } = "0%";
    public bool IsProfitable => Profit >= 0;
}

/// <summary>
/// ViewModel for manually adding an eBay order (when sync is not available).
/// </summary>
public class ManualOrderFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Order ID")]
    [StringLength(50)]
    public string EbayOrderId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Transaction Date")]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sale Amount")]
    [Range(0.01, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal GrossAmount { get; set; }

    [Required]
    [Display(Name = "eBay Fees")]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal FeesAmount { get; set; }

    [Required]
    [Display(Name = "Shipping Cost")]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal ShippingAmount { get; set; }

    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Display(Name = "Linked Inventory Item")]
    public int? InventoryItemId { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Dropdown options
    public List<SelectListItem> InventoryOptions { get; set; } = new();

    public decimal NetAmount => GrossAmount - FeesAmount - ShippingAmount;
    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Order" : "Add Manual Order";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Add Order";
}
