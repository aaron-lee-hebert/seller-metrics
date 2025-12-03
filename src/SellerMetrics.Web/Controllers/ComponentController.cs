using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Components.Commands;
using SellerMetrics.Application.Components.DTOs;
using SellerMetrics.Application.Components.Queries;
using SellerMetrics.Application.StorageLocations.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class ComponentController : Controller
{
    private readonly ILogger<ComponentController> _logger;
    private readonly GetComponentListQueryHandler _listHandler;
    private readonly GetComponentValueQueryHandler _valueHandler;
    private readonly GetComponentTypesQueryHandler _typesHandler;
    private readonly GetLowStockComponentsQueryHandler _lowStockHandler;
    private readonly CreateComponentItemCommandHandler _createHandler;
    private readonly UpdateComponentItemCommandHandler _updateHandler;
    private readonly AdjustComponentQuantityCommandHandler _adjustHandler;
    private readonly MoveComponentCommandHandler _moveHandler;
    private readonly DeleteComponentItemCommandHandler _deleteHandler;
    private readonly GetAllStorageLocationsQueryHandler _locationsHandler;

    public ComponentController(
        ILogger<ComponentController> logger,
        GetComponentListQueryHandler listHandler,
        GetComponentValueQueryHandler valueHandler,
        GetComponentTypesQueryHandler typesHandler,
        GetLowStockComponentsQueryHandler lowStockHandler,
        CreateComponentItemCommandHandler createHandler,
        UpdateComponentItemCommandHandler updateHandler,
        AdjustComponentQuantityCommandHandler adjustHandler,
        MoveComponentCommandHandler moveHandler,
        DeleteComponentItemCommandHandler deleteHandler,
        GetAllStorageLocationsQueryHandler locationsHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _valueHandler = valueHandler;
        _typesHandler = typesHandler;
        _lowStockHandler = lowStockHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _adjustHandler = adjustHandler;
        _moveHandler = moveHandler;
        _deleteHandler = deleteHandler;
        _locationsHandler = locationsHandler;
    }

    public async Task<IActionResult> Index(
        int? type,
        int? location,
        ComponentStatus? status,
        string? filter,
        int page = 1)
    {
        var showLowStockOnly = filter?.Equals("lowstock", StringComparison.OrdinalIgnoreCase) ?? false;

        var viewModel = new ComponentListViewModel
        {
            TypeFilter = type,
            LocationFilter = location,
            StatusFilter = status,
            ShowLowStockOnly = showLowStockOnly
        };

        try
        {
            // Get items based on filters
            IReadOnlyList<ComponentItemDto> items;

            if (showLowStockOnly)
            {
                items = await _lowStockHandler.HandleAsync(
                    new GetLowStockComponentsQuery(), CancellationToken.None);
            }
            else
            {
                items = await _listHandler.HandleAsync(
                    new GetComponentListQuery(type, location, status), CancellationToken.None);
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
                BaseUrl = Url.Action("Index") ?? "/Component",
                QueryParameters = BuildQueryParameters(type, location, status, filter)
            };

            // Get summary
            viewModel.Summary = await _valueHandler.HandleAsync(
                new GetComponentValueQuery(), CancellationToken.None);

            // Build filter options
            viewModel.TypeOptions = await GetTypeOptionsAsync(type);
            viewModel.LocationOptions = await GetLocationOptionsAsync(location);
            viewModel.StatusOptions = GetStatusOptions(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading component list");
            TempData["ErrorMessage"] = "Failed to load components.";
        }

        ViewData["Title"] = "Components";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var items = await _listHandler.HandleAsync(
                new GetComponentListQuery(), CancellationToken.None);
            var item = items.FirstOrDefault(i => i.Id == id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Component not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ComponentDetailViewModel
            {
                Item = item,
                LocationOptions = await GetLocationOptionsAsync(item.StorageLocationId)
            };

            ViewData["Title"] = item.Description;
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Components</a></li><li class=\"breadcrumb-item active\">{item.ComponentTypeName}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading component {Id}", id);
            TempData["ErrorMessage"] = "Failed to load component.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new ComponentFormViewModel
        {
            AcquiredDate = DateTime.Today,
            TypeOptions = await GetTypeOptionsAsync(null),
            LocationOptions = await GetLocationOptionsAsync(null),
            SourceOptions = GetSourceOptions(null)
        };

        ViewData["Title"] = "Add Component";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComponentFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.TypeOptions = await GetTypeOptionsAsync(model.ComponentTypeId);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.SourceOptions = GetSourceOptions(model.Source);
            return View(model);
        }

        try
        {
            var command = new CreateComponentItemCommand(
                model.ComponentTypeId,
                model.Description,
                model.Quantity,
                model.UnitCostAmount,
                model.UnitCostCurrency,
                model.StorageLocationId,
                model.AcquiredDate,
                model.Source,
                model.Notes,
                model.LowStockThreshold);

            var result = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = $"Component '{result.Description}' created successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating component");
            ModelState.AddModelError("", ex.Message);
            model.TypeOptions = await GetTypeOptionsAsync(model.ComponentTypeId);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.SourceOptions = GetSourceOptions(model.Source);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var items = await _listHandler.HandleAsync(
                new GetComponentListQuery(), CancellationToken.None);
            var item = items.FirstOrDefault(i => i.Id == id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Component not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ComponentFormViewModel
            {
                Id = item.Id,
                ComponentTypeId = item.ComponentTypeId,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitCostAmount = item.UnitCostAmount,
                UnitCostCurrency = item.UnitCostCurrency,
                StorageLocationId = item.StorageLocationId,
                AcquiredDate = item.AcquiredDate,
                Source = item.Source,
                Notes = item.Notes,
                LowStockThreshold = item.LowStockThreshold,
                TypeOptions = await GetTypeOptionsAsync(item.ComponentTypeId),
                LocationOptions = await GetLocationOptionsAsync(item.StorageLocationId),
                SourceOptions = GetSourceOptions(item.Source)
            };

            ViewData["Title"] = $"Edit: {item.Description}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Components</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{item.ComponentTypeName}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading component for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load component.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ComponentFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.TypeOptions = await GetTypeOptionsAsync(model.ComponentTypeId);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.SourceOptions = GetSourceOptions(model.Source);
            return View(model);
        }

        try
        {
            var command = new UpdateComponentItemCommand(
                id,
                model.ComponentTypeId,
                model.Description,
                model.UnitCostAmount,
                model.UnitCostCurrency,
                model.StorageLocationId,
                model.AcquiredDate,
                model.Source,
                model.Notes,
                model.LowStockThreshold);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Component updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating component {Id}", id);
            ModelState.AddModelError("", ex.Message);
            model.TypeOptions = await GetTypeOptionsAsync(model.ComponentTypeId);
            model.LocationOptions = await GetLocationOptionsAsync(model.StorageLocationId);
            model.SourceOptions = GetSourceOptions(model.Source);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustQuantity(int id, int adjustment, string reason)
    {
        try
        {
            await _adjustHandler.HandleAsync(
                new AdjustComponentQuantityCommand(id, adjustment, reason), CancellationToken.None);

            var action = adjustment > 0 ? "increased" : "decreased";
            TempData["SuccessMessage"] = $"Quantity {action} by {Math.Abs(adjustment)}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting component quantity {Id}", id);
            TempData["ErrorMessage"] = $"Failed to adjust quantity: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(int id, int? newLocationId)
    {
        try
        {
            await _moveHandler.HandleAsync(
                new MoveComponentCommand(id, newLocationId), CancellationToken.None);
            TempData["SuccessMessage"] = "Component moved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving component {Id}", id);
            TempData["ErrorMessage"] = $"Failed to move component: {ex.Message}";
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
                new DeleteComponentItemCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Component deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting component {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete component: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Helper Methods

    private static Dictionary<string, string> BuildQueryParameters(
        int? type,
        int? location,
        ComponentStatus? status,
        string? filter)
    {
        var parameters = new Dictionary<string, string>();
        if (type.HasValue)
            parameters["type"] = type.Value.ToString();
        if (location.HasValue)
            parameters["location"] = location.Value.ToString();
        if (status.HasValue)
            parameters["status"] = ((int)status.Value).ToString();
        if (!string.IsNullOrWhiteSpace(filter))
            parameters["filter"] = filter;
        return parameters;
    }

    private async Task<List<SelectListItem>> GetTypeOptionsAsync(int? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Types" }
        };

        try
        {
            var types = await _typesHandler.HandleAsync(
                new GetComponentTypesQuery(), CancellationToken.None);

            foreach (var type in types)
            {
                options.Add(new SelectListItem
                {
                    Value = type.Id.ToString(),
                    Text = type.Name,
                    Selected = selected == type.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading component types");
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

    private static List<SelectListItem> GetStatusOptions(ComponentStatus? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Statuses" }
        };

        foreach (ComponentStatus status in Enum.GetValues<ComponentStatus>())
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

    private static List<SelectListItem> GetSourceOptions(ComponentSource? selected)
    {
        var options = new List<SelectListItem>();

        foreach (ComponentSource source in Enum.GetValues<ComponentSource>())
        {
            options.Add(new SelectListItem
            {
                Value = ((int)source).ToString(),
                Text = source.ToString(),
                Selected = selected == source
            });
        }

        return options;
    }

    #endregion
}
