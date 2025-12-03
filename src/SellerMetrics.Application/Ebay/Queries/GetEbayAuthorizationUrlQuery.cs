using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Application.Ebay.Interfaces;

namespace SellerMetrics.Application.Ebay.Queries;

/// <summary>
/// Query to get the eBay OAuth authorization URL.
/// </summary>
public record GetEbayAuthorizationUrlQuery(string UserId);

/// <summary>
/// Handler for GetEbayAuthorizationUrlQuery.
/// </summary>
public class GetEbayAuthorizationUrlQueryHandler
{
    private readonly IEbayApiClient _ebayApiClient;

    public GetEbayAuthorizationUrlQueryHandler(IEbayApiClient ebayApiClient)
    {
        _ebayApiClient = ebayApiClient;
    }

    public Task<EbayAuthorizationDto> HandleAsync(
        GetEbayAuthorizationUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        // Generate a state parameter for CSRF protection
        // Include the user ID to verify on callback
        var state = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{query.UserId}:{Guid.NewGuid()}"));

        var authorizationUrl = _ebayApiClient.GetAuthorizationUrl(state);

        return Task.FromResult(new EbayAuthorizationDto
        {
            AuthorizationUrl = authorizationUrl,
            State = state
        });
    }
}
