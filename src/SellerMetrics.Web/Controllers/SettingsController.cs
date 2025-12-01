using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellerMetrics.Application.Wave.Commands;
using SellerMetrics.Application.Wave.Queries;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

/// <summary>
/// Controller for managing application settings including API connections.
/// </summary>
[Authorize]
public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;
    private readonly GetWaveConnectionStatusQueryHandler _waveConnectionStatusHandler;
    private readonly GetWaveBusinessesQueryHandler _waveBusinessesHandler;
    private readonly ConnectWaveAccountCommandHandler _connectWaveHandler;
    private readonly DisconnectWaveAccountCommandHandler _disconnectWaveHandler;
    private readonly SyncInvoicesFromWaveCommandHandler _syncWaveHandler;

    public SettingsController(
        ILogger<SettingsController> logger,
        GetWaveConnectionStatusQueryHandler waveConnectionStatusHandler,
        GetWaveBusinessesQueryHandler waveBusinessesHandler,
        ConnectWaveAccountCommandHandler connectWaveHandler,
        DisconnectWaveAccountCommandHandler disconnectWaveHandler,
        SyncInvoicesFromWaveCommandHandler syncWaveHandler)
    {
        _logger = logger;
        _waveConnectionStatusHandler = waveConnectionStatusHandler;
        _waveBusinessesHandler = waveBusinessesHandler;
        _connectWaveHandler = connectWaveHandler;
        _disconnectWaveHandler = disconnectWaveHandler;
        _syncWaveHandler = syncWaveHandler;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var viewModel = new SettingsViewModel();

        try
        {
            // Get Wave connection status
            var waveStatus = await _waveConnectionStatusHandler.HandleAsync(
                new GetWaveConnectionStatusQuery(userId), CancellationToken.None);

            viewModel.WaveConnection = new WaveConnectionViewModel
            {
                IsConnected = waveStatus.IsConnected,
                BusinessName = waveStatus.BusinessName,
                BusinessId = waveStatus.BusinessId,
                ConnectedAt = waveStatus.ConnectedAt,
                LastSyncedAt = waveStatus.LastSyncedAt,
                LastSyncError = waveStatus.LastSyncError
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings");
            TempData["ErrorMessage"] = "Failed to load settings.";
        }

        ViewData["Title"] = "API Connections";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidateWaveToken(string accessToken)
    {
        try
        {
            var businesses = await _waveBusinessesHandler.HandleAsync(
                new GetWaveBusinessesQuery(accessToken), CancellationToken.None);

            if (businesses.Count == 0)
            {
                return Json(new { success = false, error = "No businesses found for this token. Please verify your Wave Full Access Token." });
            }

            return Json(new
            {
                success = true,
                businesses = businesses.Select(b => new
                {
                    id = b.Id,
                    name = b.Name,
                    isPersonal = b.IsPersonal,
                    currency = b.Currency
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Wave token validation failed");
            return Json(new { success = false, error = "Invalid token. Please check your Wave Full Access Token and try again." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConnectWave(WaveConnectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide all required fields.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var result = await _connectWaveHandler.HandleAsync(
                new ConnectWaveAccountCommand(
                    userId,
                    model.AccessToken,
                    model.BusinessId,
                    model.BusinessName),
                CancellationToken.None);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Successfully connected to Wave business: {model.BusinessName}";

                // Trigger initial sync
                try
                {
                    var syncResult = await _syncWaveHandler.HandleAsync(
                        new SyncInvoicesFromWaveCommand(userId), CancellationToken.None);
                    TempData["SuccessMessage"] += $" Synced {syncResult.InvoicesSynced} invoices.";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Initial Wave sync failed after connection");
                    TempData["WarningMessage"] = "Connected to Wave, but initial sync failed. Try syncing manually.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = result.Error ?? "Failed to connect Wave account.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting Wave account");
            TempData["ErrorMessage"] = "An error occurred while connecting to Wave.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisconnectWave()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var result = await _disconnectWaveHandler.HandleAsync(
                new DisconnectWaveAccountCommand(userId), CancellationToken.None);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Wave account disconnected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error ?? "Failed to disconnect Wave account.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting Wave account");
            TempData["ErrorMessage"] = "An error occurred while disconnecting from Wave.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SyncWaveNow()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var result = await _syncWaveHandler.HandleAsync(
                new SyncInvoicesFromWaveCommand(userId), CancellationToken.None);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Wave sync completed. {result.InvoicesSynced} invoices synced, {result.InvoicesSkipped} skipped.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Wave sync failed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing Wave invoices");
            TempData["ErrorMessage"] = "An error occurred during Wave sync.";
        }

        return RedirectToAction(nameof(Index));
    }
}
