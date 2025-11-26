using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellerMetrics.Application.Mileage.Commands;
using SellerMetrics.Application.Mileage.DTOs;
using SellerMetrics.Application.Mileage.Queries;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.ValueObjects;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for IRS-compliant mileage log tracking.
/// </summary>
[Authorize]
public class MileageController : Controller
{
    private readonly ILogger<MileageController> _logger;
    private readonly GetMileageLogQueryHandler _listHandler;
    private readonly GetMileageEntryQueryHandler _getHandler;
    private readonly CalculateMileageDeductionQueryHandler _deductionHandler;
    private readonly GetIrsMileageRatesQueryHandler _ratesHandler;
    private readonly CreateMileageEntryCommandHandler _createHandler;
    private readonly UpdateMileageEntryCommandHandler _updateHandler;
    private readonly DeleteMileageEntryCommandHandler _deleteHandler;

    public MileageController(
        ILogger<MileageController> logger,
        GetMileageLogQueryHandler listHandler,
        GetMileageEntryQueryHandler getHandler,
        CalculateMileageDeductionQueryHandler deductionHandler,
        GetIrsMileageRatesQueryHandler ratesHandler,
        CreateMileageEntryCommandHandler createHandler,
        UpdateMileageEntryCommandHandler updateHandler,
        DeleteMileageEntryCommandHandler deleteHandler)
    {
        _logger = logger;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _deductionHandler = deductionHandler;
        _ratesHandler = ratesHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    public async Task<IActionResult> Index(
        BusinessLine? businessLine,
        DateTime? startDate,
        DateTime? endDate,
        int page = 1)
    {
        // Default to current year
        var today = DateTime.Today;
        var yearStart = new DateTime(today.Year, 1, 1);
        startDate ??= yearStart;
        endDate ??= today;

        var viewModel = new MileageListViewModel
        {
            BusinessLineFilter = businessLine,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            // Get entries
            var entries = await _listHandler.HandleAsync(
                new GetMileageLogQuery(startDate, endDate.Value.AddDays(1), businessLine),
                CancellationToken.None);

            // Apply pagination
            const int pageSize = 20;
            var totalItems = entries.Count;
            var pagedItems = entries
                .OrderByDescending(e => e.TripDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Entries = pagedItems;
            viewModel.Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                BaseUrl = Url.Action("Index") ?? "/Mileage",
                QueryParameters = BuildQueryParameters(businessLine, startDate, endDate)
            };

            // Calculate deduction for the period
            viewModel.Deduction = await _deductionHandler.HandleAsync(
                new CalculateMileageDeductionQuery(startDate.Value, endDate.Value.AddDays(1)),
                CancellationToken.None);

            // Get current IRS rate
            var rates = await _ratesHandler.HandleAsync(
                new GetIrsMileageRatesQuery(), CancellationToken.None);
            viewModel.CurrentRate = rates.FirstOrDefault(r => r.Year == today.Year)
                                    ?? rates.OrderByDescending(r => r.Year).FirstOrDefault();

            // Build filter options
            viewModel.BusinessLineOptions = GetBusinessLineOptions(businessLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading mileage log");
            TempData["ErrorMessage"] = "Failed to load mileage log.";
        }

        ViewData["Title"] = "Mileage Log";
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var entry = await _getHandler.HandleAsync(
                new GetMileageEntryQuery(id), CancellationToken.None);

            if (entry == null)
            {
                TempData["ErrorMessage"] = "Mileage entry not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get applicable rate for deduction calculation
            var rates = await _ratesHandler.HandleAsync(
                new GetIrsMileageRatesQuery(), CancellationToken.None);
            var rate = rates.FirstOrDefault(r => r.Year == entry.TripDate.Year)
                       ?? rates.OrderByDescending(r => r.Year).FirstOrDefault();

            var deduction = entry.TotalMiles * (rate?.StandardRate ?? 0.67m);

            var viewModel = new MileageDetailViewModel
            {
                Entry = entry,
                ApplicableRate = rate,
                EstimatedDeduction = deduction,
                EstimatedDeductionFormatted = new Money(deduction, "USD").ToString()
            };

            ViewData["Title"] = $"Trip: {entry.TripDate:MMM d, yyyy}";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Mileage Log</a></li><li class=\"breadcrumb-item active\">{entry.TripDate:MMM d}</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading mileage entry {Id}", id);
            TempData["ErrorMessage"] = "Failed to load mileage entry.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        var viewModel = new MileageFormViewModel
        {
            TripDate = DateTime.Today,
            BusinessLineOptions = GetBusinessLineOptions(null)
        };

        ViewData["Title"] = "Log Trip";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MileageFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }

        try
        {
            var command = new CreateMileageEntryCommand(
                model.TripDate,
                model.Purpose,
                model.StartLocation,
                model.Destination,
                model.Miles,
                model.BusinessLine,
                model.IsRoundTrip,
                model.Notes,
                model.ServiceJobId,
                model.OdometerStart,
                model.OdometerEnd);

            var resultId = await _createHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Trip logged successfully.";
            return RedirectToAction(nameof(Details), new { id = resultId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mileage entry");
            ModelState.AddModelError("", ex.Message);
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var entry = await _getHandler.HandleAsync(
                new GetMileageEntryQuery(id), CancellationToken.None);

            if (entry == null)
            {
                TempData["ErrorMessage"] = "Mileage entry not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new MileageFormViewModel
            {
                Id = entry.Id,
                TripDate = entry.TripDate,
                Purpose = entry.Purpose,
                StartLocation = entry.StartLocation,
                Destination = entry.Destination,
                Miles = entry.Miles,
                IsRoundTrip = entry.IsRoundTrip,
                BusinessLine = entry.BusinessLine,
                OdometerStart = entry.OdometerStart,
                OdometerEnd = entry.OdometerEnd,
                Notes = entry.Notes,
                ServiceJobId = entry.ServiceJobId,
                BusinessLineOptions = GetBusinessLineOptions(entry.BusinessLine)
            };

            ViewData["Title"] = "Edit Trip";
            ViewData["Breadcrumb"] = $"<li class=\"breadcrumb-item\"><a href=\"{Url.Action("Index")}\">Mileage Log</a></li><li class=\"breadcrumb-item\"><a href=\"{Url.Action("Details", new { id })}\">{entry.TripDate:MMM d}</a></li><li class=\"breadcrumb-item active\">Edit</li>";
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading mileage entry for edit {Id}", id);
            TempData["ErrorMessage"] = "Failed to load mileage entry.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MileageFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.BusinessLineOptions = GetBusinessLineOptions(model.BusinessLine);
            return View(model);
        }

        try
        {
            var command = new UpdateMileageEntryCommand(
                id,
                model.TripDate,
                model.Purpose,
                model.StartLocation,
                model.Destination,
                model.Miles,
                model.BusinessLine,
                model.IsRoundTrip,
                model.Notes,
                model.ServiceJobId,
                model.OdometerStart,
                model.OdometerEnd);

            await _updateHandler.HandleAsync(command, CancellationToken.None);
            TempData["SuccessMessage"] = "Trip updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating mileage entry {Id}", id);
            ModelState.AddModelError("", ex.Message);
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
                new DeleteMileageEntryCommand(id), CancellationToken.None);
            TempData["SuccessMessage"] = "Trip deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting mileage entry {Id}", id);
            TempData["ErrorMessage"] = $"Failed to delete trip: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Helper Methods

    private static Dictionary<string, string> BuildQueryParameters(
        BusinessLine? businessLine,
        DateTime? startDate,
        DateTime? endDate)
    {
        var parameters = new Dictionary<string, string>();
        if (businessLine.HasValue)
            parameters["businessLine"] = ((int)businessLine.Value).ToString();
        if (startDate.HasValue)
            parameters["startDate"] = startDate.Value.ToString("yyyy-MM-dd");
        if (endDate.HasValue)
            parameters["endDate"] = endDate.Value.ToString("yyyy-MM-dd");
        return parameters;
    }

    private static List<SelectListItem> GetBusinessLineOptions(BusinessLine? selected)
    {
        var options = new List<SelectListItem>
        {
            new() { Value = "", Text = "All Business Lines" }
        };

        foreach (BusinessLine line in Enum.GetValues<BusinessLine>())
        {
            var displayText = line == BusinessLine.eBay ? "eBay" : line.ToString();
            options.Add(new SelectListItem
            {
                Value = ((int)line).ToString(),
                Text = displayText,
                Selected = selected == line
            });
        }

        return options;
    }

    #endregion
}
