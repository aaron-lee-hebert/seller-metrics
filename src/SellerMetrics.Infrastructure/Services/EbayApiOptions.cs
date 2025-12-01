namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Configuration options for eBay API integration.
/// </summary>
public class EbayApiOptions
{
    public const string SectionName = "EbayApi";

    /// <summary>
    /// The eBay Developer App Client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The eBay Developer App Client Secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The redirect URI for OAuth callback.
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// The eBay environment (Sandbox or Production).
    /// </summary>
    public string Environment { get; set; } = "Sandbox";

    /// <summary>
    /// OAuth scopes to request.
    /// </summary>
    public string[] Scopes { get; set; } = new[]
    {
        "https://api.ebay.com/oauth/api_scope",
        "https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly",
        "https://api.ebay.com/oauth/api_scope/sell.finances.readonly"
    };

    /// <summary>
    /// Gets whether this is the production environment.
    /// </summary>
    public bool IsProduction => Environment.Equals("Production", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the base URL for the eBay authorization endpoint.
    /// </summary>
    public string AuthorizationBaseUrl => IsProduction
        ? "https://auth.ebay.com/oauth2/authorize"
        : "https://auth.sandbox.ebay.com/oauth2/authorize";

    /// <summary>
    /// Gets the base URL for the eBay OAuth token endpoint.
    /// </summary>
    public string TokenBaseUrl => IsProduction
        ? "https://api.ebay.com/identity/v1/oauth2/token"
        : "https://api.sandbox.ebay.com/identity/v1/oauth2/token";

    /// <summary>
    /// Gets the base URL for the eBay Fulfillment API.
    /// </summary>
    public string FulfillmentApiBaseUrl => IsProduction
        ? "https://api.ebay.com/sell/fulfillment/v1"
        : "https://api.sandbox.ebay.com/sell/fulfillment/v1";

    /// <summary>
    /// Gets the base URL for the eBay Identity API.
    /// </summary>
    public string IdentityApiBaseUrl => IsProduction
        ? "https://apiz.ebay.com/commerce/identity/v1"
        : "https://apiz.sandbox.ebay.com/commerce/identity/v1";
}
