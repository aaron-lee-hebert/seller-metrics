using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.StorageLocations.Commands;
using SellerMetrics.Application.StorageLocations.DTOs;
using SellerMetrics.Application.StorageLocations.Queries;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for managing storage locations (hierarchical organization of inventory).
/// </summary>
[Authorize]
public class StorageLocationController : Controller
{
    private readonly ILogger<StorageLocationController> _logger;
    private readonly GetStorageLocationHierarchyQueryHandler _hierarchyHandler;
    private readonly GetStorageLocationQueryHandler _getHandler;
    private readonly GetAllStorageLocationsQueryHandler _listHandler;
    private readonly CreateStorageLocationCommandHandler _createHandler;
    private readonly UpdateStorageLocationCommandHandler _updateHandler;
    private readonly DeleteStorageLocationCommandHandler _deleteHandler;

    public StorageLocationController(
        ILogger<StorageLocationController> logger,
        GetStorageLocationHierarchyQueryHandler hierarchyHandler,
        GetStorageLocationQueryHandler getHandler,
        GetAllStorageLocationsQueryHandler listHandler,
        CreateStorageLocationCommandHandler createHandler,
        UpdateStorageLocationCommandHandler updateHandler,
        DeleteStorageLocationCommandHandler deleteHandler)
    {
        _logger = logger;
        _hierarchyHandler = hierarchyHandler;
        _getHandler = getHandler;
        _listHandler = listHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new StorageLocationListViewModel();

        try
        {
            // Get hierarchical structure
            var hierarchy = await _hierarchyHandler.HandleAsync(
                new GetStorageLocationHierarchyQuery(),
                CancellationToken.None);

            viewModel.Locations = hierarchy.ToList();
            viewModel.TotalRootLocations = hierarchy.Count;
            viewModel.TotalLocations = CountAllLocations(hierarchy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading storage locations");
            TempData["ErrorMessage"] = "Failed to load storage locations.";
        }

        ViewData["Title"] = "Storage Locations";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var location = await _getHandler.HandleAsync(
                new GetStorageLocationQuery(id), CancellationToken.None);

            if (location == null)
            {
                TempData["ErrorMessage"] = "Storage location not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get full hierarchy to find children
            var hierarchy = await _hierarchyHandler.HandleAsync(
                new GetStorageLocationHierarchyQuery(),
                CancellationToken.None);

            var children = FindChildren(hierarchy, id);

            var viewModel = new StorageLocationDetailViewModel
            {
                Location = location,
                Children = children,
                Ancestors = BuildAncestors(hierarchy, location)
            };

            ViewData["Title"] = location.Name;
            ViewData["Breadcrumb"] = BuildBreadcrumb(viewModel.Ancestors, location);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading storage location {Id}", id);
            TempData["ErrorMessage"] = "Failed to load storage location.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create(int? parentId)
    {
        var viewModel = new StorageLocationFormViewModel
        {
            ParentId = parentId,
            ParentOptions = await GetParentOptionsAsync(null)
        };

        ViewData["Title"] = "Add Storage Location";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StorageLocationFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ParentOptions = await GetParentOptionsAsync(null);
            return View(model);
        }

        try
        {
            var command = new CreateStorageLocationCommand(
                model.Name,
                model.Description,
                model.ParentId,
                model.SortOrder);

            var result = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Storage location added successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating storage location");
            ModelState.AddModelError("", ex.Message);
            model.ParentOptions = await GetParentOptionsAsync(null);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var location = await _getHandler.HandleAsync(
                new GetStorageLocationQuery(id), CancellationToken.None);

            if (location == null)
            {
                TempData["ErrorMessage"] = "Storage location not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new StorageLocationFormViewModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                ParentId = location.ParentId,
                SortOrder = location.SortOrder,
                ParentOptions = await GetParentOptionsAsync(id)
            };

            ViewData["Title"] = "Edit Location";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Locations</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{location.Name}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading storage location for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load storage location.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StorageLocationFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.ParentOptions = await GetParentOptionsAsync(id);
            return View(model);
        }

        try
        {
            var command = new UpdateStorageLocationCommand(
                id,
                model.Name,
                model.Description,
                model.ParentId,
                model.SortOrder);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Storage location updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating storage location {Id}", id);
            ModelState.AddModelError("", ex.Message);
            model.ParentOptions = await GetParentOptionsAsync(id);
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
                new DeleteStorageLocationCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Storage location deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting storage location {Id}", id);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Helper Methods

    private static int CountAllLocations(IReadOnlyList<StorageLocationDto> locations)
    {
        int count = locations.Count;
        foreach (var location in locations)
        {
            count += CountAllLocations(location.Children);
        }
        return count;
    }

    private static List<StorageLocationDto> FindChildren(IReadOnlyList<StorageLocationDto> hierarchy, int parentId)
    {
        foreach (var location in hierarchy)
        {
            if (location.Id == parentId)
            {
                return location.Children;
            }
            var children = FindChildren(location.Children, parentId);
            if (children.Any())
            {
                return children;
            }
        }
        return new List<StorageLocationDto>();
    }

    private static StorageLocationDto? FindLocation(IReadOnlyList<StorageLocationDto> hierarchy, int id)
    {
        foreach (var location in hierarchy)
        {
            if (location.Id == id)
            {
                return location;
            }
            var found = FindLocation(location.Children, id);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private static List<StorageLocationDto> BuildAncestors(IReadOnlyList<StorageLocationDto> hierarchy, StorageLocationDto location)
    {
        var ancestors = new List<StorageLocationDto>();
        if (location.ParentId.HasValue)
        {
            BuildAncestorPath(hierarchy, location.ParentId.Value, ancestors);
        }
        return ancestors;
    }

    private static bool BuildAncestorPath(IReadOnlyList<StorageLocationDto> hierarchy, int targetId, List<StorageLocationDto> path)
    {
        foreach (var location in hierarchy)
        {
            if (location.Id == targetId)
            {
                path.Insert(0, location);
                if (location.ParentId.HasValue)
                {
                    BuildAncestorPath(hierarchy, location.ParentId.Value, path);
                }
                return true;
            }
            if (BuildAncestorPath(location.Children, targetId, path))
            {
                return true;
            }
        }
        return false;
    }

    private string BuildBreadcrumb(List<StorageLocationDto> ancestors, StorageLocationDto current)
    {
        var parts = new List<string>
        {
            $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Locations</a></li>"
        };

        foreach (var ancestor in ancestors)
        {
            parts.Add($"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id = ancestor.Id })}\">{ancestor.Name}</a></li>");
        }

        parts.Add($"<li class=\"breadcrumb-item active\">{current.Name}</li>");

        return string.Join("", parts);
    }

    private async Task<List<SelectListItem>> GetParentOptionsAsync(int? excludeId)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "(No Parent - Top Level)" }
        };

        try
        {
            var allLocations = await _listHandler.HandleAsync(
                new GetAllStorageLocationsQuery(),
                CancellationToken.None);

            foreach (var location in allLocations.OrderBy(l => l.FullPath))
            {
                // Don't include self or descendants in parent options
                if (excludeId.HasValue && location.Id == excludeId.Value)
                {
                    continue;
                }

                var indent = new string('\u00A0', location.Depth * 4); // Non-breaking spaces for indent
                options.Add(new SelectListItem
                {
                    Value = location.Id.ToString(),
                    Text = $"{indent}{location.Name}"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading parent options");
        }

        return options;
    }

    #endregion
}
