using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellerMetrics.Application.Ebay.Commands;
using SellerMetrics.Application.Ebay.Queries;
using SellerMetrics.Web.Models;

namespace SellerMetrics.Web.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;
    private readonly GetEbayConnectionStatusQueryHandler _connectionStatusHandler;
    private readonly GetEbayAuthorizationUrlQueryHandler _authorizationUrlHandler;
    private readonly ConnectEbayAccountCommandHandler _connectHandler;
    private readonly DisconnectEbayAccountCommandHandler _disconnectHandler;

    public SettingsController(
        ILogger<SettingsController> logger,
        GetEbayConnectionStatusQueryHandler connectionStatusHandler,
        GetEbayAuthorizationUrlQueryHandler authorizationUrlHandler,
        ConnectEbayAccountCommandHandler connectHandler,
        DisconnectEbayAccountCommandHandler disconnectHandler)
    {
        _logger = logger;
        _connectionStatusHandler = connectionStatusHandler;
        _authorizationUrlHandler = authorizationUrlHandler;
        _connectHandler = connectHandler;
        _disconnectHandler = disconnectHandler;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();

        var viewModel = new EbayConnectionViewModel();

        try
        {
            viewModel.Status = await _connectionStatusHandler.HandleAsync(
                new GetEbayConnectionStatusQuery(userId), CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading eBay connection status");
            viewModel.ErrorMessage = "Failed to load connection status.";
        }

        ViewData["Title"] = "Settings";
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConnectEbay()
    {
        var userId = GetUserId();

        try
        {
            var authDto = await _authorizationUrlHandler.HandleAsync(
                new GetEbayAuthorizationUrlQuery(userId), CancellationToken.None);

            // Store the state in session for validation on callback
            HttpContext.Session.SetString("EbayOAuthState", authDto.State);

            return Redirect(authDto.AuthorizationUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating eBay OAuth flow");
            TempData["ErrorMessage"] = "Failed to initiate eBay connection.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Route("settings/ebay/callback")]
    public async Task<IActionResult> EbayCallback(string code, string state, string? error = null)
    {
        if (!string.IsNullOrEmpty(error))
        {
            _logger.LogWarning("eBay OAuth error: {Error}", error);
            TempData["ErrorMessage"] = $"eBay authorization failed: {error}";
            return RedirectToAction(nameof(Index));
        }

        // Validate state parameter (CSRF protection)
        var savedState = HttpContext.Session.GetString("EbayOAuthState");
        if (string.IsNullOrEmpty(savedState) || savedState != state)
        {
            _logger.LogWarning("eBay OAuth state mismatch");
            TempData["ErrorMessage"] = "Invalid state parameter. Please try connecting again.";
            return RedirectToAction(nameof(Index));
        }

        // Clear the state from session
        HttpContext.Session.Remove("EbayOAuthState");

        // Validate the state contains the correct user ID
        try
        {
            var decodedState = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(state));
            var stateParts = decodedState.Split(':');
            var stateUserId = stateParts[0];

            var currentUserId = GetUserId();
            if (stateUserId != currentUserId)
            {
                _logger.LogWarning("eBay OAuth user ID mismatch: expected {Expected}, got {Actual}",
                    currentUserId, stateUserId);
                TempData["ErrorMessage"] = "User verification failed. Please try connecting again.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to decode eBay OAuth state");
            TempData["ErrorMessage"] = "Invalid state parameter. Please try connecting again.";
            return RedirectToAction(nameof(Index));
        }

        var userId = GetUserId();

        try
        {
            await _connectHandler.HandleAsync(
                new ConnectEbayAccountCommand(userId, code), CancellationToken.None);

            TempData["SuccessMessage"] = "Successfully connected your eBay account!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing eBay OAuth flow");
            TempData["ErrorMessage"] = $"Failed to connect eBay account: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisconnectEbay()
    {
        var userId = GetUserId();

        try
        {
            var result = await _disconnectHandler.HandleAsync(
                new DisconnectEbayAccountCommand(userId), CancellationToken.None);

            if (result)
            {
                TempData["SuccessMessage"] = "Successfully disconnected your eBay account.";
            }
            else
            {
                TempData["WarningMessage"] = "No eBay account was connected.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting eBay account");
            TempData["ErrorMessage"] = "Failed to disconnect eBay account.";
        }

        return RedirectToAction(nameof(Index));
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID not found in claims.");
    }
}
