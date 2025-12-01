namespace SellerMetrics.Application.Ebay.DTOs;

/// <summary>
/// Response from eBay OAuth token endpoint.
/// </summary>
public class EbayTokenResponse
{
    /// <summary>
    /// The access token for API calls.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token for obtaining new access tokens.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// The number of seconds until the access token expires.
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// The number of seconds until the refresh token expires.
    /// </summary>
    public int RefreshTokenExpiresIn { get; set; }

    /// <summary>
    /// The token type (typically "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// The scopes granted by the user.
    /// </summary>
    public string Scope { get; set; } = string.Empty;
}

/// <summary>
/// eBay user identity information.
/// </summary>
public class EbayUserIdentity
{
    /// <summary>
    /// The user's eBay user ID.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The user's eBay username.
    /// </summary>
    public string Username { get; set; } = string.Empty;
}

/// <summary>
/// DTO representing an order from the eBay API.
/// </summary>
public class EbayOrderDto
{
    /// <summary>
    /// The eBay order ID.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// The legacy order ID.
    /// </summary>
    public string? LegacyOrderId { get; set; }

    /// <summary>
    /// When the order was created.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// The buyer's eBay username.
    /// </summary>
    public string BuyerUsername { get; set; } = string.Empty;

    /// <summary>
    /// Line items in the order.
    /// </summary>
    public List<EbayOrderLineItemDto> LineItems { get; set; } = new();

    /// <summary>
    /// The order status.
    /// </summary>
    public string OrderStatus { get; set; } = string.Empty;

    /// <summary>
    /// The payment status.
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// The fulfillment status.
    /// </summary>
    public string FulfillmentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Order total price.
    /// </summary>
    public EbayMoneyDto Total { get; set; } = new();

    /// <summary>
    /// Delivery cost paid by buyer.
    /// </summary>
    public EbayMoneyDto? DeliveryCost { get; set; }

    /// <summary>
    /// Total fees collected by eBay for this order.
    /// </summary>
    public EbayOrderFeesDto? Fees { get; set; }
}

/// <summary>
/// DTO representing a line item in an eBay order.
/// </summary>
public class EbayOrderLineItemDto
{
    /// <summary>
    /// The line item ID.
    /// </summary>
    public string LineItemId { get; set; } = string.Empty;

    /// <summary>
    /// The eBay item ID.
    /// </summary>
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// The item title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The seller's SKU.
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Quantity purchased.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per item.
    /// </summary>
    public EbayMoneyDto LineItemCost { get; set; } = new();
}

/// <summary>
/// DTO representing monetary values from eBay.
/// </summary>
public class EbayMoneyDto
{
    /// <summary>
    /// The monetary value.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// The currency code (e.g., "USD").
    /// </summary>
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// DTO representing fees from an eBay order.
/// </summary>
public class EbayOrderFeesDto
{
    /// <summary>
    /// Final value fee.
    /// </summary>
    public EbayMoneyDto? FinalValueFee { get; set; }

    /// <summary>
    /// Payment processing fee.
    /// </summary>
    public EbayMoneyDto? PaymentProcessingFee { get; set; }

    /// <summary>
    /// Any additional fees (promoted listings, etc.).
    /// </summary>
    public EbayMoneyDto? AdditionalFees { get; set; }
}
