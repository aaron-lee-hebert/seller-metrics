using SellerMetrics.Application.Ebay.DTOs;

namespace SellerMetrics.Application.Ebay.Interfaces;

/// <summary>
/// Interface for eBay API client operations.
/// </summary>
public interface IEbayApiClient
{
    /// <summary>
    /// Generates the eBay OAuth authorization URL.
    /// </summary>
    /// <param name="state">The state parameter for CSRF protection.</param>
    /// <returns>The authorization URL to redirect the user to.</returns>
    string GetAuthorizationUrl(string state);

    /// <summary>
    /// Exchanges an authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="authorizationCode">The authorization code from eBay callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The token response containing access and refresh tokens.</returns>
    Task<EbayTokenResponse> ExchangeCodeForTokensAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an expired access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token (decrypted).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new token response.</returns>
    Task<EbayTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders from eBay for the authenticated user.
    /// </summary>
    /// <param name="accessToken">The access token (decrypted).</param>
    /// <param name="startDate">The start date for orders.</param>
    /// <param name="endDate">The end date for orders.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of eBay orders.</returns>
    Task<IReadOnlyList<EbayOrderDto>> GetOrdersAsync(
        string accessToken,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authenticated user's eBay identity information.
    /// </summary>
    /// <param name="accessToken">The access token (decrypted).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's eBay identity.</returns>
    Task<EbayUserIdentity> GetUserIdentityAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the access token is still valid by making a test API call.
    /// </summary>
    /// <param name="accessToken">The access token (decrypted).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}
