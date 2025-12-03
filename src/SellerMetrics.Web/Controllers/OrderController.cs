using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Inventory.Queries;
using SellerMetrics.Application.Revenue.Commands;
using SellerMetrics.Application.Revenue.DTOs;
using SellerMetrics.Application.Revenue.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for eBay orders (revenue entries with eBay source).
/// This is a placeholder UI until eBay API sync is implemented.
/// </summary>
[Authorize]
public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly GetRevenueListQueryHandler _listHandler;
    private readonly GetRevenueEntryQueryHandler _getHandler;
    private readonly CreateRevenueEntryCommandHandler _createHandler;
    private readonly UpdateRevenueEntryCommandHandler _updateHandler;
    private readonly DeleteRevenueEntryCommandHandler _deleteHandler;
    private readonly GetInventoryListQueryHandler _inventoryHandler;

    public OrderController(
        ILogger<OrderController> logger,
        GetRevenueListQueryHandler listHandler,
        GetRevenueEntryQueryHandler getHandler,
        CreateRevenueEntryCommandHandler createHandler,
        UpdateRevenueEntryCommandHandler updateHandler,
        DeleteRevenueEntryCommandHandler deleteHandler,
        GetInventoryListQueryHandler inventoryHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _inventoryHandler = inventoryHandler;
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int page = 1)
    {
        // Default to current quarter
        var today = DateTime.Today;
        var quarterStart = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
        startDate ??= quarterStart;
        endDate ??= today;

        var viewModel = new OrderListViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            IsEbaySyncConfigured = false // TODO: Check if eBay API is configured
        };

        try
        {
            var orders = await _listHandler.HandleAsync(
                new GetRevenueListQuery(RevenueSource.eBay, startDate, endDate.Value.AddDays(1)),
                CancellationToken.None);

            // Apply pagination
            const int pageSize = 20;
            var totalItems = orders.Count;
            var pagedItems = orders
                .OrderByDescending(o => o.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Orders = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/Order",
                QueryParameters = BuildQueryParameters(startDate, endDate)
            };

            // Calculate summary
            viewModel.Summary = new OrderSummaryViewModel
            {
                TotalOrders = orders.Count,
                TotalGross = orders.Sum(o => o.GrossAmount),
                TotalGrossFormatted = new Money(orders.Sum(o => o.GrossAmount), "USD").ToString(),
                TotalFees = orders.Sum(o => o.FeesAmount),
                TotalFeesFormatted = new Money(orders.Sum(o => o.FeesAmount), "USD").ToString(),
                TotalShipping = orders.Sum(o => o.ShippingAmount),
                TotalShippingFormatted = new Money(orders.Sum(o => o.ShippingAmount), "USD").ToString(),
                TotalNet = orders.Sum(o => o.NetAmount),
                TotalNetFormatted = new Money(orders.Sum(o => o.NetAmount), "USD").ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading eBay orders");
            TempData["ErrorMessage"] = "Failed to load orders.";
        }

        ViewData["Title"] = "eBay Orders";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var order = await _getHandler.HandleAsync(
                new GetRevenueEntryQuery(id), CancellationToken.None);

            if (order == null || order.Source != RevenueSource.eBay)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new OrderDetailViewModel
            {
                Order = order
            };

            // Calculate profit if linked to inventory
            if (order.InventoryItemId.HasValue)
            {
                var inventoryItems = await _inventoryHandler.HandleAsync(
                    new GetInventoryListQuery(), CancellationToken.None);
                var inventoryItem = inventoryItems.FirstOrDefault(i => i.Id == order.InventoryItemId);

                if (inventoryItem != null)
                {
                    viewModel.HasCostData = true;
                    viewModel.CostOfGoods = inventoryItem.CogsAmount;
                    viewModel.CostOfGoodsFormatted = inventoryItem.CogsFormatted;
                    viewModel.Profit = order.NetAmount - inventoryItem.CogsAmount;
                    viewModel.ProfitFormatted = new Money(viewModel.Profit, order.Currency).ToString();
                    viewModel.ProfitMargin = order.GrossAmount > 0
                        ? (viewModel.Profit / order.GrossAmount) * 100
                        : 0;
                    viewModel.ProfitMarginFormatted = $"{viewModel.ProfitMargin:F1}%";
                }
            }

            ViewData["Title"] = $"Order: {order.EbayOrderId ?? order.Description}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">eBay Orders</a></li><li class=\"breadcrumb-item active\">{order.EbayOrderId ?? $"#{order.Id}"}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order {Id}", id);
            TempData["ErrorMessage"] = "Failed to load order.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new ManualOrderFormViewModel
        {
            TransactionDate = DateTime.Today,
            InventoryOptions = await GetInventoryOptionsAsync(null)
        };

        ViewData["Title"] = "Add Manual Order";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ManualOrderFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.InventoryOptions = await GetInventoryOptionsAsync(model.InventoryItemId);
            return View(model);
        }

        try
        {
            var command = new CreateRevenueEntryCommand(
                RevenueSource.eBay,
                model.TransactionDate,
                model.Description,
                model.GrossAmount,
                model.FeesAmount,
                model.ShippingAmount,
                0, // No taxes collected for eBay orders
                model.Currency,
                model.EbayOrderId,
                null, // WaveInvoiceNumber
                model.InventoryItemId,
                null, // ServiceJobId
                model.Notes);

            var resultId = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Order added successfully.";
            return RedirectToAction(nameof(Details), new { id = resultId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating manual order");
            ModelState.AddModelError("", ex.Message);
            model.InventoryOptions = await GetInventoryOptionsAsync(model.InventoryItemId);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var order = await _getHandler.HandleAsync(
                new GetRevenueEntryQuery(id), CancellationToken.None);

            if (order == null || order.Source != RevenueSource.eBay)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            // Only allow editing manual entries
            if (order.EntryType != RevenueEntryType.Manual)
            {
                TempData["WarningMessage"] = "Synced orders cannot be edited. Changes will be overwritten on next sync.";
            }

            var viewModel = new ManualOrderFormViewModel
            {
                Id = order.Id,
                EbayOrderId = order.EbayOrderId ?? string.Empty,
                TransactionDate = order.TransactionDate,
                Description = order.Description,
                GrossAmount = order.GrossAmount,
                FeesAmount = order.FeesAmount,
                ShippingAmount = order.ShippingAmount,
                Currency = order.Currency,
                InventoryItemId = order.InventoryItemId,
                Notes = order.Notes,
                InventoryOptions = await GetInventoryOptionsAsync(order.InventoryItemId)
            };

            ViewData["Title"] = "Edit Order";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">eBay Orders</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{order.EbayOrderId ?? $"#{order.Id}"}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load order.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ManualOrderFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.InventoryOptions = await GetInventoryOptionsAsync(model.InventoryItemId);
            return View(model);
        }

        try
        {
            var command = new UpdateRevenueEntryCommand(
                id,
                RevenueSource.eBay,
                model.TransactionDate,
                model.Description,
                model.GrossAmount,
                model.FeesAmount,
                model.ShippingAmount,
                0, // No taxes collected for eBay orders
                model.Currency,
                model.EbayOrderId,
                null, // WaveInvoiceNumber
                model.InventoryItemId,
                null, // ServiceJobId
                model.Notes);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Order updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {Id}", id);
            ModelState.AddModelError("", ex.Message);
            model.InventoryOptions = await GetInventoryOptionsAsync(model.InventoryItemId);
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
            TempData["SuccessMessage"] = "Order deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete order: {ex.Message}";
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

    private async Task<List<SelectListItem>> GetInventoryOptionsAsync(int? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "Not Linked" }
        };

        try
        {
            var items = await _inventoryHandler.HandleAsync(
                new GetInventoryListQuery(InventoryStatus.Sold), CancellationToken.None);

            foreach (var item in items.OrderByDescending(i => i.SoldDate))
            {
                options.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.EffectiveSku} - {item.Title}",
                    Selected = selected == item.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory items");
        }

        return options;
    }

    #endregion
}
