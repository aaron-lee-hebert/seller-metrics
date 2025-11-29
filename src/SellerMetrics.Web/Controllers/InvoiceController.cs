using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellerMetrics.Application.Revenue.Commands;
using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Application.Revenue.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for service invoices (revenue entries with ComputerServices source).
/// This is a placeholder UI until Wave API sync is implemented.
/// </summary>
[Authorize]
public class InvoiceController : Controller
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly GetRevenueListQueryHandler _listHandler;
    private readonly GetRevenueEntryQueryHandler _getHandler;
    private readonly CreateRevenueEntryCommandHandler _createHandler;
    private readonly UpdateRevenueEntryCommandHandler _updateHandler;
    private readonly DeleteRevenueEntryCommandHandler _deleteHandler;

    public InvoiceController(
        ILogger<InvoiceController> logger,
        GetRevenueListQueryHandler listHandler,
        GetRevenueEntryQueryHandler getHandler,
        CreateRevenueEntryCommandHandler createHandler,
        UpdateRevenueEntryCommandHandler updateHandler,
        DeleteRevenueEntryCommandHandler deleteHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int page = 1)
    {
        // Default to current quarter
        var today = DateTime.Today;
        var quarterStart = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
        startDate ??= quarterStart;
        endDate ??= today;

        var viewModel = new InvoiceListViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            IsWaveSyncConfigured = false // TODO: Check if Wave API is configured
        };

        try
        {
            var invoices = await _listHandler.HandleAsync(
                new GetRevenueListQuery(RevenueSource.ComputerServices, startDate, endDate.Value.AddDays(1)),
                CancellationToken.None);

            // Apply pagination
            const int pageSize = 20;
            var totalItems = invoices.Count;
            var pagedItems = invoices
                .OrderByDescending(i => i.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Invoices = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/Invoice",
                QueryParameters = BuildQueryParameters(startDate, endDate)
            };

            // Calculate summary
            viewModel.Summary = new InvoiceSummaryViewModel
            {
                TotalInvoices = invoices.Count,
                TotalRevenue = invoices.Sum(i => i.GrossAmount),
                TotalRevenueFormatted = new Money(invoices.Sum(i => i.GrossAmount), "USD").ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service invoices");
            TempData["ErrorMessage"] = "Failed to load invoices.";
        }

        ViewData["Title"] = "Service Invoices";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var invoice = await _getHandler.HandleAsync(
                new GetRevenueEntryQuery(id), CancellationToken.None);

            if (invoice == null || invoice.Source != RevenueSource.ComputerServices)
            {
                TempData["ErrorMessage"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new InvoiceDetailViewModel
            {
                Invoice = invoice
            };

            ViewData["Title"] = $"Invoice: {invoice.WaveInvoiceNumber ?? invoice.Description}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Invoices</a></li><li class=\"breadcrumb-item active\">{invoice.WaveInvoiceNumber ?? $"#{invoice.Id}"}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice {Id}", id);
            TempData["ErrorMessage"] = "Failed to load invoice.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        var viewModel = new ManualInvoiceFormViewModel
        {
            TransactionDate = DateTime.Today
        };

        ViewData["Title"] = "Add Manual Invoice";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ManualInvoiceFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new CreateRevenueEntryCommand(
                RevenueSource.ComputerServices,
                model.TransactionDate,
                model.Description,
                model.GrossAmount,
                0, // No fees for service invoices
                0, // No shipping for service invoices
                model.TaxesCollectedAmount,
                model.Currency,
                null, // EbayOrderId
                model.WaveInvoiceNumber,
                null, // InventoryItemId
                null, // ServiceJobId
                model.Notes);

            var resultId = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Invoice added successfully.";
            return RedirectToAction(nameof(Details), new { id = resultId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating manual invoice");
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var invoice = await _getHandler.HandleAsync(
                new GetRevenueEntryQuery(id), CancellationToken.None);

            if (invoice == null || invoice.Source != RevenueSource.ComputerServices)
            {
                TempData["ErrorMessage"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            // Only allow editing manual entries
            if (invoice.EntryType != RevenueEntryType.Manual)
            {
                TempData["WarningMessage"] = "Synced invoices cannot be edited. Changes will be overwritten on next sync.";
            }

            var viewModel = new ManualInvoiceFormViewModel
            {
                Id = invoice.Id,
                WaveInvoiceNumber = invoice.WaveInvoiceNumber ?? string.Empty,
                TransactionDate = invoice.TransactionDate,
                Description = invoice.Description,
                GrossAmount = invoice.GrossAmount,
                TaxesCollectedAmount = invoice.TaxesCollectedAmount,
                Currency = invoice.Currency,
                Notes = invoice.Notes
            };

            ViewData["Title"] = "Edit Invoice";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Invoices</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{invoice.WaveInvoiceNumber ?? $"#{invoice.Id}"}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load invoice.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ManualInvoiceFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new UpdateRevenueEntryCommand(
                id,
                RevenueSource.ComputerServices,
                model.TransactionDate,
                model.Description,
                model.GrossAmount,
                0, // No fees
                0, // No shipping
                model.TaxesCollectedAmount,
                model.Currency,
                null, // EbayOrderId
                model.WaveInvoiceNumber,
                null, // InventoryItemId
                null, // ServiceJobId
                model.Notes);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Invoice updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice {Id}", id);
            ModelState.AddModelError("", ex.Message);
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
                new DeleteRevenueEntryCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Invoice deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete invoice: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Helper Methods

    private static Dictionary<string, string> BuildQueryParameters(DateTime? startDate, DateTime? endDate)
    {
        var parameters = new Dictionary<string, string>();
        if (startDate.HasValue)
            parameters["startDate"] = startDate.Value.ToString("yyyy-MM-dd");
        if (endDate.HasValue)
            parameters["endDate"] = endDate.Value.ToString("yyyy-MM-dd");
        return parameters;
    }

    #endregion
}
