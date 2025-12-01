using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Application.Ebay.Interfaces;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Ebay.Commands;

/// <summary>
/// Command to sync orders from eBay for a specific user.
/// </summary>
public record SyncOrdersFromEbayCommand(
    string UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null);

/// <summary>
/// Result of syncing orders from eBay.
/// </summary>
public class SyncOrdersResult
{
    public int OrdersSynced { get; set; }
    public int OrdersCreated { get; set; }
    public int OrdersUpdated { get; set; }
    public int OrdersLinked { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool Success => Errors.Count == 0;
}

/// <summary>
/// Handler for SyncOrdersFromEbayCommand.
/// </summary>
public class SyncOrdersFromEbayCommandHandler
{
    private readonly IEbayApiClient _ebayApiClient;
    private readonly IEbayUserCredentialRepository _credentialRepository;
    private readonly IEbayOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public SyncOrdersFromEbayCommandHandler(
        IEbayApiClient ebayApiClient,
        IEbayUserCredentialRepository credentialRepository,
        IEbayOrderRepository orderRepository,
        IInventoryItemRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ITokenEncryptionService tokenEncryptionService)
    {
        _ebayApiClient = ebayApiClient;
        _credentialRepository = credentialRepository;
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _tokenEncryptionService = tokenEncryptionService;
    }

    public async Task<SyncOrdersResult> HandleAsync(
        SyncOrdersFromEbayCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = new SyncOrdersResult();

        // Get user credentials
        var credential = await _credentialRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (credential == null || !credential.IsConnected)
        {
            result.Errors.Add("eBay account is not connected.");
            return result;
        }

        // Check if refresh token is expired
        if (credential.IsRefreshTokenExpired)
        {
            credential.IsConnected = false;
            credential.RecordSyncError("Refresh token expired. Please reconnect your eBay account.");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            result.Errors.Add("eBay refresh token has expired. Please reconnect your account.");
            return result;
        }

        try
        {
            // Get or refresh access token
            var accessToken = await GetValidAccessTokenAsync(credential, cancellationToken);

            // Default to last 30 days if no date range specified
            var startDate = command.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = command.EndDate ?? DateTime.UtcNow;

            // Fetch orders from eBay
            var ebayOrders = await _ebayApiClient.GetOrdersAsync(accessToken, startDate, endDate, cancellationToken);

            foreach (var ebayOrder in ebayOrders)
            {
                try
                {
                    await ProcessOrderAsync(ebayOrder, command.UserId, result, cancellationToken);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error processing order {ebayOrder.OrderId}: {ex.Message}");
                }
            }

            // Record successful sync
            credential.RecordSuccessfulSync();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.OrdersSynced = ebayOrders.Count;
        }
        catch (Exception ex)
        {
            credential.RecordSyncError(ex.Message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            result.Errors.Add($"Sync failed: {ex.Message}");
        }

        return result;
    }

    private async Task<string> GetValidAccessTokenAsync(
        EbayUserCredential credential,
        CancellationToken cancellationToken)
    {
        // Check if access token needs refresh
        if (credential.IsAccessTokenExpired)
        {
            var refreshToken = _tokenEncryptionService.Decrypt(credential.EncryptedRefreshToken);
            var tokenResponse = await _ebayApiClient.RefreshAccessTokenAsync(refreshToken, cancellationToken);

            // Update tokens
            credential.UpdateTokens(
                _tokenEncryptionService.Encrypt(tokenResponse.AccessToken),
                DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                !string.IsNullOrEmpty(tokenResponse.RefreshToken)
                    ? _tokenEncryptionService.Encrypt(tokenResponse.RefreshToken)
                    : null,
                tokenResponse.RefreshTokenExpiresIn > 0
                    ? DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn)
                    : null);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return tokenResponse.AccessToken;
        }

        return _tokenEncryptionService.Decrypt(credential.EncryptedAccessToken);
    }

    private async Task ProcessOrderAsync(
        EbayOrderDto ebayOrder,
        string userId,
        SyncOrdersResult result,
        CancellationToken cancellationToken)
    {
        // Check if order already exists
        var existingOrder = await _orderRepository.GetByEbayOrderIdAsync(ebayOrder.OrderId, userId, cancellationToken);

        if (existingOrder != null)
        {
            // Update existing order
            UpdateExistingOrder(existingOrder, ebayOrder);
            result.OrdersUpdated++;
        }
        else
        {
            // Create new order
            var newOrder = await CreateNewOrderAsync(ebayOrder, userId, result, cancellationToken);
            result.OrdersCreated++;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void UpdateExistingOrder(EbayOrder order, EbayOrderDto ebayOrder)
    {
        order.UpdateFromSync(
            ParseOrderStatus(ebayOrder.OrderStatus),
            ParsePaymentStatus(ebayOrder.PaymentStatus),
            ParseFulfillmentStatus(ebayOrder.FulfillmentStatus),
            ebayOrder.Fees?.FinalValueFee != null
                ? new Money(ebayOrder.Fees.FinalValueFee.Value, ebayOrder.Fees.FinalValueFee.Currency)
                : null,
            ebayOrder.Fees?.PaymentProcessingFee != null
                ? new Money(ebayOrder.Fees.PaymentProcessingFee.Value, ebayOrder.Fees.PaymentProcessingFee.Currency)
                : null,
            ebayOrder.Fees?.AdditionalFees != null
                ? new Money(ebayOrder.Fees.AdditionalFees.Value, ebayOrder.Fees.AdditionalFees.Currency)
                : null);
    }

    private async Task<EbayOrder> CreateNewOrderAsync(
        EbayOrderDto ebayOrder,
        string userId,
        SyncOrdersResult result,
        CancellationToken cancellationToken)
    {
        // Get first line item (most orders have one item)
        var lineItem = ebayOrder.LineItems.FirstOrDefault();

        var order = new EbayOrder
        {
            UserId = userId,
            EbayOrderId = ebayOrder.OrderId,
            LegacyOrderId = ebayOrder.LegacyOrderId,
            OrderDate = ebayOrder.OrderDate,
            BuyerUsername = ebayOrder.BuyerUsername,
            ItemTitle = lineItem?.Title ?? "Unknown Item",
            EbayItemId = lineItem?.ItemId,
            Sku = lineItem?.Sku,
            Quantity = lineItem?.Quantity ?? 1,
            GrossSale = new Money(ebayOrder.Total.Value, ebayOrder.Total.Currency),
            ShippingPaid = ebayOrder.DeliveryCost != null
                ? new Money(ebayOrder.DeliveryCost.Value, ebayOrder.DeliveryCost.Currency)
                : Money.Zero(),
            ShippingActual = Money.Zero(),
            FinalValueFee = ebayOrder.Fees?.FinalValueFee != null
                ? new Money(ebayOrder.Fees.FinalValueFee.Value, ebayOrder.Fees.FinalValueFee.Currency)
                : Money.Zero(),
            PaymentProcessingFee = ebayOrder.Fees?.PaymentProcessingFee != null
                ? new Money(ebayOrder.Fees.PaymentProcessingFee.Value, ebayOrder.Fees.PaymentProcessingFee.Currency)
                : Money.Zero(),
            AdditionalFees = ebayOrder.Fees?.AdditionalFees != null
                ? new Money(ebayOrder.Fees.AdditionalFees.Value, ebayOrder.Fees.AdditionalFees.Currency)
                : Money.Zero(),
            Status = ParseOrderStatus(ebayOrder.OrderStatus),
            PaymentStatus = ParsePaymentStatus(ebayOrder.PaymentStatus),
            FulfillmentStatus = ParseFulfillmentStatus(ebayOrder.FulfillmentStatus),
            LastSyncedAt = DateTime.UtcNow
        };

        // Try to link to inventory item by SKU
        if (!string.IsNullOrEmpty(lineItem?.Sku))
        {
            var inventoryItem = await _inventoryRepository.GetByEbaySkuAsync(lineItem.Sku, cancellationToken)
                ?? await _inventoryRepository.GetByInternalSkuAsync(lineItem.Sku, cancellationToken);

            if (inventoryItem != null)
            {
                order.LinkToInventory(inventoryItem.Id);

                // Mark inventory as sold if not already
                if (inventoryItem.Status != InventoryStatus.Sold)
                {
                    inventoryItem.MarkAsSold();
                }

                result.OrdersLinked++;
            }
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        return order;
    }

    private static EbayOrderStatus ParseOrderStatus(string status) => status.ToUpperInvariant() switch
    {
        "ACTIVE" => EbayOrderStatus.Active,
        "COMPLETED" => EbayOrderStatus.Completed,
        "CANCELLED" => EbayOrderStatus.Cancelled,
        "INACTIVE" => EbayOrderStatus.Inactive,
        _ => EbayOrderStatus.Active
    };

    private static EbayPaymentStatus ParsePaymentStatus(string status) => status.ToUpperInvariant() switch
    {
        "PENDING" => EbayPaymentStatus.Pending,
        "FAILED" => EbayPaymentStatus.Failed,
        "PAID" => EbayPaymentStatus.Paid,
        "PARTIALLY_REFUNDED" => EbayPaymentStatus.PartiallyRefunded,
        "FULLY_REFUNDED" => EbayPaymentStatus.FullyRefunded,
        _ => EbayPaymentStatus.Pending
    };

    private static EbayFulfillmentStatus ParseFulfillmentStatus(string status) => status.ToUpperInvariant() switch
    {
        "NOT_STARTED" => EbayFulfillmentStatus.NotStarted,
        "IN_PROGRESS" => EbayFulfillmentStatus.InProgress,
        "FULFILLED" => EbayFulfillmentStatus.Fulfilled,
        _ => EbayFulfillmentStatus.NotStarted
    };
}
