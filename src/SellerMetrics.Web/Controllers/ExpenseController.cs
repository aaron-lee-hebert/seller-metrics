using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Expenses.Commands;
using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Application.Expenses.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly ILogger<ExpenseController> _logger;
    private readonly GetExpensesByDateRangeQueryHandler _listHandler;
    private readonly GetExpenseSummaryQueryHandler _summaryHandler;
    private readonly GetExpenseQueryHandler _getHandler;
    private readonly CreateExpenseCommandHandler _createHandler;
    private readonly UpdateExpenseCommandHandler _updateHandler;
    private readonly DeleteExpenseCommandHandler _deleteHandler;

    public ExpenseController(
        ILogger<ExpenseController> logger,
        GetExpensesByDateRangeQueryHandler listHandler,
        GetExpenseSummaryQueryHandler summaryHandler,
        GetExpenseQueryHandler getHandler,
        CreateExpenseCommandHandler createHandler,
        UpdateExpenseCommandHandler updateHandler,
        DeleteExpenseCommandHandler deleteHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _summaryHandler = summaryHandler;
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    public async Task<IActionResult> Index(
        ExpenseCategory? category,
        BusinessLine? businessLine,
        DateTime? startDate,
        DateTime? endDate,
        int page = 1)
    {
        // Default to current quarter
        var today = DateTime.Today;
        var quarterStart = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
        startDate ??= quarterStart;
        endDate ??= today;

        var viewModel = new ExpenseListViewModel
        {
            CategoryFilter = category,
            BusinessLineFilter = businessLine,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            // Get expenses by date range first
            var expenses = await _listHandler.HandleAsync(
                new GetExpensesByDateRangeQuery(startDate.Value, endDate.Value.AddDays(1)),
                CancellationToken.None);

            // Apply additional filters in memory
            IEnumerable<BusinessExpenseDto> filtered = expenses;
            if (category.HasValue)
            {
                filtered = filtered.Where(e => e.Category == category.Value);
            }
            if (businessLine.HasValue)
            {
                filtered = filtered.Where(e => e.BusinessLine == businessLine.Value);
            }

            // Apply pagination
            const int pageSize = 20;
            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;
            var pagedItems = filteredList
                .OrderByDescending(e => e.ExpenseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Expenses = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/Expense",
                QueryParameters = BuildQueryParameters(category, businessLine, startDate, endDate)
            };

            // Get summary
            viewModel.Summary = await _summaryHandler.HandleAsync(
                new GetExpenseSummaryQuery(startDate.Value, endDate.Value.AddDays(1)),
                CancellationToken.None);

            // Build filter options
            viewModel.CategoryOptions = GetCategoryOptions(category);
            viewModel.BusinessLineOptions = GetBusinessLineOptions(businessLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expenses");
            TempData["ErrorMessage"] = "Failed to load expenses.";
        }

        ViewData["Title"] = "Business Expenses";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var expense = await _getHandler.HandleAsync(
                new GetExpenseQuery(id), CancellationToken.None);

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ExpenseDetailViewModel
            {
                Expense = expense
            };

            ViewData["Title"] = $"Expense: {expense.Description}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Expenses</a></li><li class=\"breadcrumb-item active\">{expense.ExpenseDate:MMM d, yyyy}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expense {Id}", id);
            TempData["ErrorMessage"] = "Failed to load expense.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        var viewModel = new ExpenseFormViewModel
        {
            ExpenseDate = DateTime.Today,
            CategoryOptions = GetCategoryOptions(null),
            BusinessLineOptions = GetBusinessLineOptions(null)
        };

        ViewData["Title"] = "Log Expense";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.CategoryOptions = GetCategoryOptions(model.Category);
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }

        try
        {
            var command = new CreateExpenseCommand(
                model.ExpenseDate,
                model.Description,
                model.Amount,
                model.Category,
                model.BusinessLine,
                model.Currency,
                model.Vendor,
                model.ReceiptPath,
                model.Notes,
                null, // ServiceJobId
                model.IsTaxDeductible,
                model.ReferenceNumber);

            var resultId = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Expense logged successfully.";
            return RedirectToAction(nameof(Details), new { id = resultId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            ModelState.AddModelError("", ex.Message);
            model.CategoryOptions = GetCategoryOptions(model.Category);
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var expense = await _getHandler.HandleAsync(
                new GetExpenseQuery(id), CancellationToken.None);

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ExpenseFormViewModel
            {
                Id = expense.Id,
                ExpenseDate = expense.ExpenseDate,
                Description = expense.Description,
                Amount = expense.Amount,
                Currency = expense.Currency,
                Category = expense.Category,
                BusinessLine = expense.BusinessLine,
                Vendor = expense.Vendor,
                ReferenceNumber = expense.ReferenceNumber,
                IsTaxDeductible = expense.IsTaxDeductible,
                ReceiptPath = expense.ReceiptPath,
                Notes = expense.Notes,
                CategoryOptions = GetCategoryOptions(expense.Category),
                BusinessLineOptions = GetBusinessLineOptions(expense.BusinessLine)
            };

            ViewData["Title"] = "Edit Expense";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Expenses</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{expense.ExpenseDate:MMM d}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expense for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load expense.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.CategoryOptions = GetCategoryOptions(model.Category);
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }

        try
        {
            var command = new UpdateExpenseCommand(
                id,
                model.ExpenseDate,
                model.Description,
                model.Amount,
                model.Category,
                model.BusinessLine,
                model.Currency,
                model.Vendor,
                model.ReceiptPath,
                model.Notes,
                null, // ServiceJobId
                model.IsTaxDeductible,
                model.ReferenceNumber);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Expense updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expense {Id}", id);
            ModelState.AddModelError("", ex.Message);
            model.CategoryOptions = GetCategoryOptions(model.Category);
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deleteHandler.HandleAsync(
                new DeleteExpenseCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Expense deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expense {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete expense: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Helper Methods

    private static Dictionary<string, string> BuildQueryParameters(
        ExpenseCategory? category,
        BusinessLine? businessLine,
        DateTime? startDate,
        DateTime? endDate)
    {
        var parameters = new Dictionary<string, string>();
        if (category.HasValue)
            parameters["category"] = ((int)category.Value).ToString();
        if (businessLine.HasValue)
            parameters["businessLine"] = ((int)businessLine.Value).ToString();
        if (startDate.HasValue)
            parameters["startDate"] = startDate.Value.ToString("yyyy-MM-dd");
        if (endDate.HasValue)
            parameters["endDate"] = endDate.Value.ToString("yyyy-MM-dd");
        return parameters;
    }

    private static List<SelectListItem> GetCategoryOptions(ExpenseCategory? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Categories" }
        };

        foreach (ExpenseCategory category in Enum.GetValues<ExpenseCategory>())
        {
            options.Add(new SelectListItem
            {
                Value = ((int)category).ToString(),
                Text = $"Line {(int)category}: {category}",
                Selected = selected == category
            });
        }

        return options;
    }

    private static List<SelectListItem> GetBusinessLineOptions(BusinessLine? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Business Lines" }
        };

        foreach (BusinessLine line in Enum.GetValues<BusinessLine>())
        {
            options.Add(new SelectListItem
            {
                Value = ((int)line).ToString(),
                Text = line.ToString(),
                Selected = selected == line
            });
        }

        return options;
    }

    #endregion
}
