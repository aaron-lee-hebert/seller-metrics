using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Inventory.Commands;
using SellerMetrics.Application.Inventory.Queries;
using SellerMetrics.Application.StorageLocations.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class InventoryController : Controller
{
    private readonly ILogger<InventoryController> _logger;
    private readonly GetInventoryListQueryHandler _listHandler;
    private readonly GetInventoryItemQueryHandler _getHandler;
    private readonly GetInventoryValueQueryHandler _valueHandler;
    private readonly SearchInventoryQueryHandler _searchHandler;
    private readonly CreateInventoryItemCommandHandler _createHandler;
    private readonly UpdateInventoryItemCommandHandler _updateHandler;
    private readonly MoveInventoryItemCommandHandler _moveHandler;
    private readonly SellInventoryItemCommandHandler _sellHandler;
    private readonly DeleteInventoryItemCommandHandler _deleteHandler;
    private readonly GetAllStorageLocationsQueryHandler _locationsHandler;

    public InventoryController(
        ILogger<InventoryController> logger,
        GetInventoryListQueryHandler listHandler,
        GetInventoryItemQueryHandler getHandler,
        GetInventoryValueQueryHandler valueHandler,
        SearchInventoryQueryHandler searchHandler,
        CreateInventoryItemCommandHandler createHandler,
        UpdateInventoryItemCommandHandler updateHandler,
        MoveInventoryItemCommandHandler moveHandler,
        SellInventoryItemCommandHandler sellHandler,
        DeleteInventoryItemCommandHandler deleteHandler,
        GetAllStorageLocationsQueryHandler locationsHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _valueHandler = valueHandler;
        _searchHandler = searchHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _moveHandler = moveHandler;
        _sellHandler = sellHandler;
        _deleteHandler = deleteHandler;
        _locationsHandler = locationsHandler;
    }

    public async Task<IActionResult> Index(
        InventoryStatus? status,
        int? location,
        string? search,
        int page = 1)
    {
        var viewModel = new InventoryListViewModel
        {
            StatusFilter = status,
            LocationFilter = location,
            SearchTerm = search
        };

        try
        {
            // Get items based on filters
            IReadOnlyList<Application.Inventory.DTOs.InventoryItemDto> items;

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = await _searchHandler.HandleAsync(
                    new SearchInventoryQuery(search), CancellationToken.None);
            }
            else
            {
                items = await _listHandler.HandleAsync(
                    new GetInventoryListQuery(status, location), CancellationToken.None);
            }

            // Apply pagination
            const int pageSize = 20;
            var totalItems = items.Count;
            var pagedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Items = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/Inventory",
                QueryParameters = BuildQueryParameters(status, location, search)
            };

            // Get summary
            viewModel.Summary = await _valueHandler.HandleAsync(
                new GetInventoryValueQuery(), CancellationToken.None);

            // Build filter options
            viewModel.StatusOptions = GetStatusOptions(status);
            viewModel.LocationOptions = await GetLocationOptionsAsync(location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory list");
            TempData["ErrorMessage"] = "Failed to load inventory items.";
        }

        ViewData["Title"] = "eBay Inventory";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var item = await _getHandler.HandleAsync(
                new GetInventoryItemQuery(id), CancellationToken.None);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory item not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new InventoryDetailViewModel
            {
                Item = item,
                LocationOptions = await GetLocationOptionsAsync(item.StorageLocationId)
            };

            ViewData["Title"] = item.Title;
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Inventory</a></li><li class=\"breadcrumb-item active\">{item.EffectiveSku}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory item {Id}", id);
            TempData["ErrorMessage"] = "Failed to load inventory item.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new InventoryFormViewModel
        {
            PurchaseDate = DateTime.Today,
            StatusOptions = GetStatusOptions(null),
            LocationOptions = await GetLocationOptionsAsync(null),
            ConditionOptions = GetConditionOptions(null)
        };

        ViewData["Title"] = "Add Inventory Item";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.StatusOptions = GetStatusOptions(model.Status);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.ConditionOptions = GetConditionOptions(model.Condition);
            return View(model);
        }

        try
        {
            var command = new CreateInventoryItemCommand(
                model.Title,
                model.Description,
                model.CogsAmount,
                model.CogsCurrency,
                model.Quantity,
                model.PurchaseDate,
                model.StorageLocationId,
                model.Condition,
                model.Notes,
                model.EbaySku);

            var result = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = $"Inventory item '{result.Title}' created with SKU {result.EffectiveSku}.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory item");
            ModelState.AddModelError("", ex.Message);
            model.StatusOptions = GetStatusOptions(model.Status);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.ConditionOptions = GetConditionOptions(model.Condition);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var item = await _getHandler.HandleAsync(
                new GetInventoryItemQuery(id), CancellationToken.None);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory item not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new InventoryFormViewModel
            {
                Id = item.Id,
                EbaySku = item.EbaySku,
                Title = item.Title,
                Description = item.Description,
                CogsAmount = item.CogsAmount,
                CogsCurrency = item.CogsCurrency,
                Quantity = item.Quantity,
                PurchaseDate = item.PurchaseDate,
                StorageLocationId = item.StorageLocationId,
                Status = item.Status,
                Condition = item.Condition,
                Notes = item.Notes,
                PhotoPath = item.PhotoPath,
                StatusOptions = GetStatusOptions(item.Status),
                LocationOptions = await GetLocationOptionsAsync(item.StorageLocationId),
                ConditionOptions = GetConditionOptions(item.Condition)
            };

            ViewData["Title"] = $"Edit: {item.Title}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Inventory</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{item.EffectiveSku}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory item for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load inventory item.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.StatusOptions = GetStatusOptions(model.Status);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.ConditionOptions = GetConditionOptions(model.Condition);
            return View(model);
        }

        try
        {
            var command = new UpdateInventoryItemCommand(
                id,
                model.Title,
                model.Description,
                model.CogsAmount,
                model.CogsCurrency,
                model.Quantity,
                model.PurchaseDate,
                model.StorageLocationId,
                model.Condition,
                model.Notes,
                model.EbaySku);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Inventory item updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory item {Id}", id);
            ModelState.AddModelError("", ex.Message);
            model.StatusOptions = GetStatusOptions(model.Status);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.ConditionOptions = GetConditionOptions(model.Condition);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(int id, int? newLocationId)
    {
        try
        {
            await _moveHandler.HandleAsync(
                new MoveInventoryItemCommand(id, newLocationId), CancellationToken.None);
            TempData["SuccessMessage"] = "Inventory item moved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving inventory item {Id}", id);
            TempData["ErrorMessage"] = $"Failed to move item: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SellOne(int id)
    {
        try
        {
            var result = await _sellHandler.HandleAsync(
                new SellInventoryItemCommand(id), CancellationToken.None);

            if (result.IsCompletelySold)
            {
                TempData["SuccessMessage"] = "Inventory item marked as sold.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Sold 1 unit. {result.OriginalItem.Quantity} remaining.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selling inventory item {Id}", id);
            TempData["ErrorMessage"] = $"Failed to sell item: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deleteHandler.HandleAsync(
                new DeleteInventoryItemCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Inventory item deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inventory item {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete item: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // AJAX endpoint for quick search
    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        {
            return Json(new List<object>());
        }

        try
        {
            var items = await _searchHandler.HandleAsync(
                new SearchInventoryQuery(term), CancellationToken.None);

            var results = items.Take(10).Select(i => new
            {
                i.Id,
                i.EffectiveSku,
                i.Title,
                i.StorageLocationPath,
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

    private static Dictionary<string, string> BuildQueryParameters(
        InventoryStatus? status,
        int? location,
        string? search)
    {
        var parameters = new Dictionary<string, string>();
        if (status.HasValue)
            parameters["status"] = ((int)status.Value).ToString();
        if (location.HasValue)
            parameters["location"] = location.Value.ToString();
        if (!string.IsNullOrWhiteSpace(search))
            parameters["search"] = search;
        return parameters;
    }

    private static List<SelectListItem> GetStatusOptions(InventoryStatus? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Statuses" }
        };

        foreach (InventoryStatus status in Enum.GetValues<InventoryStatus>())
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

    private async Task<List<SelectListItem>> GetLocationOptionsAsync(int? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "No Location" }
        };

        try
        {
            var locations = await _locationsHandler.HandleAsync(
                new GetAllStorageLocationsQuery(), CancellationToken.None);

            foreach (var location in locations)
            {
                options.Add(new SelectListItem
                {
                    Value = location.Id.ToString(),
                    Text = location.FullPath,
                    Selected = selected == location.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading storage locations");
        }

        return options;
    }

    private static List<SelectListItem> GetConditionOptions(EbayCondition? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "Not Specified" }
        };

        foreach (EbayCondition condition in Enum.GetValues<EbayCondition>())
        {
            options.Add(new SelectListItem
            {
                Value = ((int)condition).ToString(),
                Text = condition.GetDisplayName(),
                Selected = selected == condition
            });
        }

        return options;
    }

    #endregion
}
