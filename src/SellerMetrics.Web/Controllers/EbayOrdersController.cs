using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Ebay.Commands;
using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Application.Ebay.Queries;
using SellerMetrics.Application.Inventory.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class EbayOrdersController : Controller
{
    private readonly ILogger<EbayOrdersController> _logger;
    private readonly GetEbayOrderListQueryHandler _listHandler;
    private readonly GetEbayOrderQueryHandler _getHandler;
    private readonly GetEbayOrderStatsQueryHandler _statsHandler;
    private readonly GetEbayConnectionStatusQueryHandler _connectionStatusHandler;
    private readonly SyncOrdersFromEbayCommandHandler _syncHandler;
    private readonly LinkOrderToInventoryCommandHandler _linkHandler;
    private readonly UpdateShippingCostCommandHandler _updateShippingHandler;
    private readonly SearchInventoryQueryHandler _searchInventoryHandler;

    public EbayOrdersController(
        ILogger<EbayOrdersController> logger,
        GetEbayOrderListQueryHandler listHandler,
        GetEbayOrderQueryHandler getHandler,
        GetEbayOrderStatsQueryHandler statsHandler,
        GetEbayConnectionStatusQueryHandler connectionStatusHandler,
        SyncOrdersFromEbayCommandHandler syncHandler,
        LinkOrderToInventoryCommandHandler linkHandler,
        UpdateShippingCostCommandHandler updateShippingHandler,
        SearchInventoryQueryHandler searchInventoryHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _statsHandler = statsHandler;
        _connectionStatusHandler = connectionStatusHandler;
        _syncHandler = syncHandler;
        _linkHandler = linkHandler;
        _updateShippingHandler = updateShippingHandler;
        _searchInventoryHandler = searchInventoryHandler;
    }

    public async Task<IActionResult> Index(
        DateTime? startDate,
        DateTime? endDate,
        EbayOrderStatus? status,
        bool? linkedOnly,
        int page = 1)
    {
        var userId = GetUserId();

        var viewModel = new EbayOrderListViewModel
        {
            StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
            EndDate = endDate ?? DateTime.UtcNow,
            StatusFilter = status,
            LinkedOnlyFilter = linkedOnly
        };

        try
        {
            // Get connection status for display
            viewModel.ConnectionStatus = await _connectionStatusHandler.HandleAsync(
                new GetEbayConnectionStatusQuery(userId), CancellationToken.None);

            if (!viewModel.ConnectionStatus.IsConnected)
            {
                TempData["WarningMessage"] = "Your eBay account is not connected. Please connect it in your profile settings to sync orders.";
            }

            // Get orders
            var orders = await _listHandler.HandleAsync(
                new GetEbayOrderListQuery(userId, viewModel.StartDate, viewModel.EndDate, status, linkedOnly),
                CancellationToken.None);

            // Apply pagination
            const int pageSize = 20;
            var totalItems = orders.Count;
            var pagedItems = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Orders = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/EbayOrders",
                QueryParameters = BuildQueryParameters(viewModel.StartDate, viewModel.EndDate, status, linkedOnly)
            };

            // Get stats for the period
            viewModel.Stats = await _statsHandler.HandleAsync(
                new GetEbayOrderStatsQuery(userId, viewModel.StartDate, viewModel.EndDate),
                CancellationToken.None);

            // Build filter options
            viewModel.StatusOptions = GetStatusOptions(status);
            viewModel.LinkedOnlyOptions = GetLinkedOnlyOptions(linkedOnly);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading eBay orders list");
            TempData["ErrorMessage"] = "Failed to load eBay orders.";
        }

        ViewData["Title"] = "eBay Orders";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = GetUserId();

        try
        {
            var order = await _getHandler.HandleAsync(
                new GetEbayOrderQuery(id, userId), CancellationToken.None);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EbayOrderDetailViewModel
            {
                Order = order
            };

            ViewData["Title"] = $"Order: {order.EbayOrderId}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">eBay Orders</a></li><li class=\"breadcrumb-item active\">{order.EbayOrderId}</li>";
            return View(viewModel);
        }
        catch (UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = "You do not have permission to view this order.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order details {Id}", id);
            TempData["ErrorMessage"] = "Failed to load order details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SyncNow()
    {
        var userId = GetUserId();

        try
        {
            var result = await _syncHandler.HandleAsync(
                new SyncOrdersFromEbayCommand(userId), CancellationToken.None);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Sync completed. {result.OrdersCreated} new orders, {result.OrdersUpdated} updated, {result.OrdersLinked} linked to inventory.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Sync completed with errors: {string.Join(", ", result.Errors)}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing eBay orders");
            TempData["ErrorMessage"] = $"Failed to sync orders: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkToInventory(int orderId, int inventoryItemId)
    {
        var userId = GetUserId();

        try
        {
            await _linkHandler.HandleAsync(
                new LinkOrderToInventoryCommand(orderId, inventoryItemId, userId), CancellationToken.None);

            TempData["SuccessMessage"] = "Order linked to inventory item successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking order {OrderId} to inventory {InventoryId}", orderId, inventoryItemId);
            TempData["ErrorMessage"] = $"Failed to link order: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShippingCost(int orderId, decimal shippingAmount, string currency = "USD")
    {
        var userId = GetUserId();

        try
        {
            await _updateShippingHandler.HandleAsync(
                new UpdateShippingCostCommand(orderId, shippingAmount, currency, userId), CancellationToken.None);

            TempData["SuccessMessage"] = "Shipping cost updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shipping cost for order {OrderId}", orderId);
            TempData["ErrorMessage"] = $"Failed to update shipping cost: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    // AJAX endpoint for searching inventory to link
    [HttpGet]
    public async Task<IActionResult> SearchInventory(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        {
            return Json(new List<object>());
        }

        try
        {
            var items = await _searchInventoryHandler.HandleAsync(
                new SearchInventoryQuery(term), CancellationToken.None);

            var results = items.Take(10).Select(i => new
            {
                i.Id,
                i.EffectiveSku,
                i.Title,
                i.CogsFormatted,
                i.StatusDisplay
            });

            return Json(results);
        }
        catch
        {
            return Json(new List<object>());
        }
    }

    #region Helper Methods

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID not found in claims.");
    }

    private static Dictionary<string, string> BuildQueryParameters(
        DateTime startDate,
        DateTime endDate,
        EbayOrderStatus? status,
        bool? linkedOnly)
    {
        var parameters = new Dictionary<string, string>
        {
            ["startDate"] = startDate.ToString("yyyy-MM-dd"),
            ["endDate"] = endDate.ToString("yyyy-MM-dd")
        };

        if (status.HasValue)
            parameters["status"] = ((int)status.Value).ToString();
        if (linkedOnly.HasValue)
            parameters["linkedOnly"] = linkedOnly.Value.ToString().ToLower();

        return parameters;
    }

    private static List<SelectListItem> GetStatusOptions(EbayOrderStatus? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Statuses" }
        };

        foreach (EbayOrderStatus status in Enum.GetValues<EbayOrderStatus>())
        {
            options.Add(new SelectListItem
            {
                Value = ((int)status).ToString(),
                Text = status.ToString(),
                Selected = selected == status
            });
        }

        return options;
    }

    private static List<SelectListItem> GetLinkedOnlyOptions(bool? selected)
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "All Orders", Selected = !selected.HasValue },
            new() { Value = "true", Text = "Linked to Inventory", Selected = selected == true },
            new() { Value = "false", Text = "Unlinked Orders", Selected = selected == false }
        };
    }

    #endregion
}
