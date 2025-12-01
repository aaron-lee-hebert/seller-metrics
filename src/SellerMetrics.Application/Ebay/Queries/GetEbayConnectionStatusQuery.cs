using SellerMetrics.Application.Ebay.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Ebay.Queries;

/// <summary>
/// Query to get the eBay connection status for a user.
/// </summary>
public record GetEbayConnectionStatusQuery(string UserId);

/// <summary>
/// Handler for GetEbayConnectionStatusQuery.
/// </summary>
public class GetEbayConnectionStatusQueryHandler
{
    private readonly IEbayUserCredentialRepository _credentialRepository;

    public GetEbayConnectionStatusQueryHandler(IEbayUserCredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<EbayConnectionStatusDto> HandleAsync(
        GetEbayConnectionStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.GetByUserIdAsync(query.UserId, cancellationToken);

        if (credential == null || !credential.IsConnected)
        {
            return new EbayConnectionStatusDto
            {
                IsConnected = false,
                EbayUsername = null,
                ConnectedAt = null,
                LastSyncedAt = null,
                LastSyncError = null,
                RequiresReauthorization = false,
                Scopes = null
            };
        }

        return new EbayConnectionStatusDto
        {
            IsConnected = credential.IsConnected,
            EbayUsername = credential.EbayUsername,
            ConnectedAt = credential.CreatedAt,
            LastSyncedAt = credential.LastSyncedAt,
            LastSyncError = credential.LastSyncError,
            RequiresReauthorization = credential.IsRefreshTokenExpired,
            Scopes = credential.Scopes
        };
    }
}
