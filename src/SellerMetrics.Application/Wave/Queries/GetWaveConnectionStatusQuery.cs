using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Queries;

/// <summary>
/// Query to get Wave connection status for a user.
/// </summary>
public record GetWaveConnectionStatusQuery(string UserId);

/// <summary>
/// Handler for GetWaveConnectionStatusQuery.
/// </summary>
public class GetWaveConnectionStatusQueryHandler
{
    private readonly IWaveUserCredentialRepository _credentialRepository;

    public GetWaveConnectionStatusQueryHandler(IWaveUserCredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<WaveConnectionStatusDto> HandleAsync(
        GetWaveConnectionStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.GetByUserIdAsync(query.UserId, cancellationToken);

        if (credential == null)
        {
            return new WaveConnectionStatusDto { IsConnected = false };
        }

        return new WaveConnectionStatusDto
        {
            IsConnected = credential.IsConnected,
            BusinessName = credential.WaveBusinessName,
            BusinessId = credential.WaveBusinessId,
            ConnectedAt = credential.ConnectedAt,
            LastSyncedAt = credential.LastSyncedAt,
            LastSyncError = credential.LastSyncError
        };
    }
}
