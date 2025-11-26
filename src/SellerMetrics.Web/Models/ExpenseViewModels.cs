using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for the expenses list page.
/// </summary>
public class ExpenseListViewModel
{
    public List<BusinessExpenseDto> Expenses { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();

    // Filters
    public ExpenseCategory? CategoryFilter { get; set; }
    public BusinessLine? BusinessLineFilter { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Filter options
    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> BusinessLineOptions { get; set; } = new();

    // Summary
    public ExpenseSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// ViewModel for creating/editing expenses.
/// </summary>
public class ExpenseFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Amount")]
    [Range(0.01, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Required]
    [Display(Name = "Category")]
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Supplies;

    [Required]
    [Display(Name = "Business Line")]
    public BusinessLine BusinessLine { get; set; } = BusinessLine.eBay;

    [Display(Name = "Vendor")]
    [StringLength(200)]
    public string? Vendor { get; set; }

    [Display(Name = "Reference Number")]
    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [Display(Name = "Tax Deductible")]
    public bool IsTaxDeductible { get; set; } = true;

    [Display(Name = "Receipt Path")]
    [StringLength(500)]
    public string? ReceiptPath { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Dropdown options
    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> BusinessLineOptions { get; set; } = new();

    public bool IsEdit => Id.HasValue;
    public string FormTitle => IsEdit ? "Edit Expense" : "Log Expense";
    public string SubmitButtonText => IsEdit ? "Save Changes" : "Log Expense";
}

/// <summary>
/// ViewModel for expense details page.
/// </summary>
public class ExpenseDetailViewModel
{
    public BusinessExpenseDto Expense { get; set; } = new();
}
