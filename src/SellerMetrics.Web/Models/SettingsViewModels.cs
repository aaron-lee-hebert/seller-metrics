using System.ComponentModel.DataAnnotations;

namespace SellerMetrics.Web.Models;

/// <summary>
/// View model for the Settings page.
/// </summary>
public class SettingsViewModel
{
    public WaveConnectionViewModel WaveConnection { get; set; } = new();
}

/// <summary>
/// View model for Wave connection status.
/// </summary>
public class WaveConnectionViewModel
{
    public bool IsConnected { get; set; }
    public string? BusinessName { get; set; }
    public string? BusinessId { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? LastSyncError { get; set; }
}

/// <summary>
/// Form model for connecting to Wave.
/// </summary>
public class WaveConnectFormViewModel
{
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a business")]
    public string BusinessId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business name is required")]
    public string BusinessName { get; set; } = string.Empty;
}
