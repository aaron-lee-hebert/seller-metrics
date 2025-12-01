namespace SellerMetrics.Application.Ebay.DTOs;

/// <summary>
/// DTO for displaying eBay connection status.
/// </summary>
public class EbayConnectionStatusDto
{
    /// <summary>
    /// Whether the user has connected their eBay account.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// The eBay username (if connected).
    /// </summary>
    public string? EbayUsername { get; set; }

    /// <summary>
    /// When the connection was established.
    /// </summary>
    public DateTime? ConnectedAt { get; set; }

    /// <summary>
    /// When orders were last synced.
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// The last sync error message (if any).
    /// </summary>
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Whether the refresh token is expired (requiring re-authentication).
    /// </summary>
    public bool RequiresReauthorization { get; set; }

    /// <summary>
    /// The scopes granted by the user.
    /// </summary>
    public string? Scopes { get; set; }
}

/// <summary>
/// DTO for initiating eBay OAuth flow.
/// </summary>
public class EbayAuthorizationDto
{
    /// <summary>
    /// The URL to redirect the user to for eBay authorization.
    /// </summary>
    public string AuthorizationUrl { get; set; } = string.Empty;

    /// <summary>
    /// The state parameter for CSRF protection.
    /// </summary>
    public string State { get; set; } = string.Empty;
}

/// <summary>
/// DTO for eBay OAuth callback.
/// </summary>
public class EbayCallbackDto
{
    /// <summary>
    /// The authorization code from eBay.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The state parameter for CSRF validation.
    /// </summary>
    public string State { get; set; } = string.Empty;
}
