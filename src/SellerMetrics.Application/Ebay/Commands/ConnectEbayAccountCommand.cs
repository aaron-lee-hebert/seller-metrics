using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Application.Ebay.Interfaces;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Ebay.Commands;

/// <summary>
/// Command to connect an eBay account using an authorization code.
/// </summary>
public record ConnectEbayAccountCommand(
    string UserId,
    string AuthorizationCode);

/// <summary>
/// Handler for ConnectEbayAccountCommand.
/// </summary>
public class ConnectEbayAccountCommandHandler
{
    private readonly IEbayApiClient _ebayApiClient;
    private readonly IEbayUserCredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public ConnectEbayAccountCommandHandler(
        IEbayApiClient ebayApiClient,
        IEbayUserCredentialRepository credentialRepository,
        IUnitOfWork unitOfWork,
        ITokenEncryptionService tokenEncryptionService)
    {
        _ebayApiClient = ebayApiClient;
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
        _tokenEncryptionService = tokenEncryptionService;
    }

    public async Task<EbayConnectionStatusDto> HandleAsync(
        ConnectEbayAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        // Exchange authorization code for tokens
        var tokenResponse = await _ebayApiClient.ExchangeCodeForTokensAsync(
            command.AuthorizationCode, cancellationToken);

        // Validate the token by getting user identity
        var userIdentity = await _ebayApiClient.GetUserIdentityAsync(
            tokenResponse.AccessToken, cancellationToken);

        // Check if user already has credentials
        var existingCredential = await _credentialRepository.GetByUserIdAsync(
            command.UserId, cancellationToken);

        if (existingCredential != null)
        {
            // Update existing credential
            existingCredential.EbayUserId = userIdentity.UserId;
            existingCredential.EbayUsername = userIdentity.Username;
            existingCredential.EncryptedAccessToken = _tokenEncryptionService.Encrypt(tokenResponse.AccessToken);
            existingCredential.EncryptedRefreshToken = _tokenEncryptionService.Encrypt(tokenResponse.RefreshToken);
            existingCredential.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            existingCredential.RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn);
            existingCredential.Scopes = tokenResponse.Scope;
            existingCredential.IsConnected = true;
            existingCredential.LastSyncError = null;
            existingCredential.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Create new credential
            var credential = new EbayUserCredential
            {
                UserId = command.UserId,
                EbayUserId = userIdentity.UserId,
                EbayUsername = userIdentity.Username,
                EncryptedAccessToken = _tokenEncryptionService.Encrypt(tokenResponse.AccessToken),
                EncryptedRefreshToken = _tokenEncryptionService.Encrypt(tokenResponse.RefreshToken),
                AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn),
                Scopes = tokenResponse.Scope,
                IsConnected = true
            };

            await _credentialRepository.AddAsync(credential, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EbayConnectionStatusDto
        {
            IsConnected = true,
            EbayUsername = userIdentity.Username,
            ConnectedAt = DateTime.UtcNow,
            LastSyncedAt = null,
            LastSyncError = null,
            RequiresReauthorization = false,
            Scopes = tokenResponse.Scope
        };
    }
}
