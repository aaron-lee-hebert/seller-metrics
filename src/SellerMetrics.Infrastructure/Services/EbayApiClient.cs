using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Application.Ebay.Interfaces;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// eBay API client implementation with OAuth 2.0 support.
/// </summary>
public class EbayApiClient : IEbayApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EbayApiOptions _options;
    private readonly ILogger<EbayApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public EbayApiClient(
        HttpClient httpClient,
        IOptions<EbayApiOptions> options,
        ILogger<EbayApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public string GetAuthorizationUrl(string state)
    {
        var scopeString = string.Join(" ", _options.Scopes);
        var encodedScopes = HttpUtility.UrlEncode(scopeString);
        var encodedRedirectUri = HttpUtility.UrlEncode(_options.RedirectUri);
        var encodedState = HttpUtility.UrlEncode(state);

        return $"{_options.AuthorizationBaseUrl}?" +
               $"client_id={_options.ClientId}&" +
               $"response_type=code&" +
               $"redirect_uri={encodedRedirectUri}&" +
               $"scope={encodedScopes}&" +
               $"state={encodedState}";
    }

    public async Task<EbayTokenResponse> ExchangeCodeForTokensAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", authorizationCode),
            new KeyValuePair<string, string>("redirect_uri", _options.RedirectUri)
        });

        return await RequestTokenAsync(content, cancellationToken);
    }

    public async Task<EbayTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var scopeString = string.Join(" ", _options.Scopes);
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("scope", scopeString)
        });

        return await RequestTokenAsync(content, cancellationToken);
    }

    public async Task<IReadOnlyList<EbayOrderDto>> GetOrdersAsync(
        string accessToken,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var orders = new List<EbayOrderDto>();
        var offset = 0;
        const int limit = 50;
        var hasMore = true;

        while (hasMore)
        {
            var filterStart = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var filterEnd = endDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var filter = $"creationdate:[{filterStart}..{filterEnd}]";

            var url = $"{_options.FulfillmentApiBaseUrl}/order?" +
                      $"filter={HttpUtility.UrlEncode(filter)}&" +
                      $"limit={limit}&" +
                      $"offset={offset}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var ordersResponse = JsonSerializer.Deserialize<EbayOrdersApiResponse>(responseContent, _jsonOptions);

            if (ordersResponse?.Orders != null)
            {
                foreach (var order in ordersResponse.Orders)
                {
                    orders.Add(MapToOrderDto(order));
                }
            }

            // Check if there are more pages
            hasMore = ordersResponse?.Next != null;
            offset += limit;

            // Safety limit
            if (offset > 1000)
            {
                _logger.LogWarning("Reached safety limit of 1000 orders during sync");
                break;
            }
        }

        return orders;
    }

    public async Task<EbayUserIdentity> GetUserIdentityAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.IdentityApiBaseUrl}/user";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var userResponse = JsonSerializer.Deserialize<EbayUserApiResponse>(responseContent, _jsonOptions);

        return new EbayUserIdentity
        {
            UserId = userResponse?.UserId ?? string.Empty,
            Username = userResponse?.Username ?? string.Empty
        };
    }

    public async Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await GetUserIdentityAsync(accessToken, cancellationToken);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    private async Task<EbayTokenResponse> RequestTokenAsync(
        HttpContent content,
        CancellationToken cancellationToken)
    {
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenBaseUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("eBay token request failed: {StatusCode} - {Content}",
                response.StatusCode, responseContent);
            throw new HttpRequestException($"eBay token request failed: {response.StatusCode}");
        }

        var tokenResponse = JsonSerializer.Deserialize<EbayTokenApiResponse>(responseContent, _jsonOptions);

        return new EbayTokenResponse
        {
            AccessToken = tokenResponse?.AccessToken ?? string.Empty,
            RefreshToken = tokenResponse?.RefreshToken ?? string.Empty,
            ExpiresIn = tokenResponse?.ExpiresIn ?? 7200,
            RefreshTokenExpiresIn = tokenResponse?.RefreshTokenExpiresIn ?? 47304000, // 18 months
            TokenType = tokenResponse?.TokenType ?? "Bearer",
            Scope = tokenResponse?.Scope ?? string.Empty
        };
    }

    private static EbayOrderDto MapToOrderDto(EbayOrderApiResponse order)
    {
        return new EbayOrderDto
        {
            OrderId = order.OrderId ?? string.Empty,
            LegacyOrderId = order.LegacyOrderId,
            OrderDate = order.CreationDate,
            BuyerUsername = order.Buyer?.Username ?? "Unknown",
            OrderStatus = order.OrderFulfillmentStatus ?? "UNKNOWN",
            PaymentStatus = order.OrderPaymentStatus ?? "UNKNOWN",
            FulfillmentStatus = order.OrderFulfillmentStatus ?? "UNKNOWN",
            Total = new EbayMoneyDto
            {
                Value = order.PricingSummary?.Total?.Value ?? 0,
                Currency = order.PricingSummary?.Total?.Currency ?? "USD"
            },
            DeliveryCost = order.PricingSummary?.DeliveryCost != null
                ? new EbayMoneyDto
                {
                    Value = order.PricingSummary.DeliveryCost.Value ?? 0,
                    Currency = order.PricingSummary.DeliveryCost.Currency ?? "USD"
                }
                : null,
            Fees = MapFeesDto(order),
            LineItems = order.LineItems?.Select(li => new EbayOrderLineItemDto
            {
                LineItemId = li.LineItemId ?? string.Empty,
                ItemId = li.LegacyItemId ?? string.Empty,
                Title = li.Title ?? string.Empty,
                Sku = li.Sku,
                Quantity = li.Quantity ?? 1,
                LineItemCost = new EbayMoneyDto
                {
                    Value = li.LineItemCost?.Value ?? 0,
                    Currency = li.LineItemCost?.Currency ?? "USD"
                }
            }).ToList() ?? new List<EbayOrderLineItemDto>()
        };
    }

    private static EbayOrderFeesDto? MapFeesDto(EbayOrderApiResponse order)
    {
        if (order.PricingSummary?.Fee == null)
            return null;

        return new EbayOrderFeesDto
        {
            FinalValueFee = new EbayMoneyDto
            {
                Value = order.PricingSummary.Fee.Value ?? 0,
                Currency = order.PricingSummary.Fee.Currency ?? "USD"
            }
        };
    }

    #region API Response Models

    private class EbayTokenApiResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

    private class EbayUserApiResponse
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }
    }

    private class EbayOrdersApiResponse
    {
        [JsonPropertyName("orders")]
        public List<EbayOrderApiResponse>? Orders { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    private class EbayOrderApiResponse
    {
        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        [JsonPropertyName("legacyOrderId")]
        public string? LegacyOrderId { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("orderFulfillmentStatus")]
        public string? OrderFulfillmentStatus { get; set; }

        [JsonPropertyName("orderPaymentStatus")]
        public string? OrderPaymentStatus { get; set; }

        [JsonPropertyName("buyer")]
        public EbayBuyerApiResponse? Buyer { get; set; }

        [JsonPropertyName("pricingSummary")]
        public EbayPricingSummaryApiResponse? PricingSummary { get; set; }

        [JsonPropertyName("lineItems")]
        public List<EbayLineItemApiResponse>? LineItems { get; set; }
    }

    private class EbayBuyerApiResponse
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }
    }

    private class EbayPricingSummaryApiResponse
    {
        [JsonPropertyName("total")]
        public EbayAmountApiResponse? Total { get; set; }

        [JsonPropertyName("deliveryCost")]
        public EbayAmountApiResponse? DeliveryCost { get; set; }

        [JsonPropertyName("fee")]
        public EbayAmountApiResponse? Fee { get; set; }
    }

    private class EbayAmountApiResponse
    {
        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    private class EbayLineItemApiResponse
    {
        [JsonPropertyName("lineItemId")]
        public string? LineItemId { get; set; }

        [JsonPropertyName("legacyItemId")]
        public string? LegacyItemId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("lineItemCost")]
        public EbayAmountApiResponse? LineItemCost { get; set; }
    }

    #endregion
}
