using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SellerMetrics.Application.Ebay.Commands;
using SellerMetrics.Application.Ebay.Interfaces;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Background job for syncing eBay orders for all connected users.
/// </summary>
public class EbayOrderSyncJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EbayOrderSyncJob> _logger;

    public EbayOrderSyncJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EbayOrderSyncJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executes the sync job for all connected users.
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting eBay order sync job");

        using var scope = _serviceScopeFactory.CreateScope();

        var credentialRepository = scope.ServiceProvider.GetRequiredService<IEbayUserCredentialRepository>();
        var syncHandler = scope.ServiceProvider.GetRequiredService<SyncOrdersFromEbayCommandHandler>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Get all connected users
        var connectedUsers = await credentialRepository.GetAllConnectedAsync();
        _logger.LogInformation("Found {Count} connected users to sync", connectedUsers.Count);

        var successCount = 0;
        var errorCount = 0;

        foreach (var credential in connectedUsers)
        {
            try
            {
                _logger.LogDebug("Syncing orders for user {UserId}", credential.UserId);

                var result = await syncHandler.HandleAsync(
                    new SyncOrdersFromEbayCommand(credential.UserId),
                    CancellationToken.None);

                if (result.Success)
                {
                    successCount++;
                    _logger.LogDebug(
                        "Sync completed for user {UserId}: {Created} created, {Updated} updated, {Linked} linked",
                        credential.UserId, result.OrdersCreated, result.OrdersUpdated, result.OrdersLinked);
                }
                else
                {
                    errorCount++;
                    _logger.LogWarning(
                        "Sync completed with errors for user {UserId}: {Errors}",
                        credential.UserId, string.Join(", ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogError(ex, "Error syncing orders for user {UserId}", credential.UserId);

                // Record the error on the credential
                try
                {
                    credential.RecordSyncError(ex.Message);
                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception saveEx)
                {
                    _logger.LogError(saveEx, "Failed to save sync error for user {UserId}", credential.UserId);
                }
            }
        }

        _logger.LogInformation(
            "eBay order sync job completed. Success: {SuccessCount}, Errors: {ErrorCount}",
            successCount, errorCount);
    }

    /// <summary>
    /// Refreshes expired access tokens for all users that need it.
    /// </summary>
    public async Task RefreshExpiredTokensAsync()
    {
        _logger.LogInformation("Starting eBay token refresh job");

        using var scope = _serviceScopeFactory.CreateScope();

        var credentialRepository = scope.ServiceProvider.GetRequiredService<IEbayUserCredentialRepository>();
        var ebayApiClient = scope.ServiceProvider.GetRequiredService<IEbayApiClient>();
        var tokenEncryptionService = scope.ServiceProvider.GetRequiredService<ITokenEncryptionService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Get users needing token refresh
        var usersNeedingRefresh = await credentialRepository.GetNeedingTokenRefreshAsync();
        _logger.LogInformation("Found {Count} users needing token refresh", usersNeedingRefresh.Count);

        foreach (var credential in usersNeedingRefresh)
        {
            try
            {
                var refreshToken = tokenEncryptionService.Decrypt(credential.EncryptedRefreshToken);
                var tokenResponse = await ebayApiClient.RefreshAccessTokenAsync(refreshToken);

                credential.UpdateTokens(
                    tokenEncryptionService.Encrypt(tokenResponse.AccessToken),
                    DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    !string.IsNullOrEmpty(tokenResponse.RefreshToken)
                        ? tokenEncryptionService.Encrypt(tokenResponse.RefreshToken)
                        : null,
                    tokenResponse.RefreshTokenExpiresIn > 0
                        ? DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn)
                        : null);

                await unitOfWork.SaveChangesAsync();

                _logger.LogDebug("Token refreshed for user {UserId}", credential.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token for user {UserId}", credential.UserId);

                // If refresh fails, mark as needing re-authorization
                credential.RecordSyncError($"Token refresh failed: {ex.Message}");
                await unitOfWork.SaveChangesAsync();
            }
        }

        // Handle users with expired refresh tokens
        var usersWithExpiredRefresh = await credentialRepository.GetWithExpiredRefreshTokensAsync();
        foreach (var credential in usersWithExpiredRefresh)
        {
            _logger.LogWarning(
                "User {UserId} has expired refresh token and needs to re-authenticate",
                credential.UserId);

            credential.IsConnected = false;
            credential.RecordSyncError("Refresh token expired. Please reconnect your eBay account.");
            await unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("eBay token refresh job completed");
    }
}
