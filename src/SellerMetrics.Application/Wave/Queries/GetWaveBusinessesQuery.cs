using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Application.Wave.Interfaces;

namespace SellerMetrics.Application.Wave.Queries;

/// <summary>
/// Query to get available Wave businesses for token validation.
/// </summary>
public record GetWaveBusinessesQuery(string AccessToken);

/// <summary>
/// Handler for GetWaveBusinessesQuery.
/// </summary>
public class GetWaveBusinessesQueryHandler
{
    private readonly IWaveApiClient _waveApiClient;

    public GetWaveBusinessesQueryHandler(IWaveApiClient waveApiClient)
    {
        _waveApiClient = waveApiClient;
    }

    public async Task<IReadOnlyList<WaveBusinessDto>> HandleAsync(
        GetWaveBusinessesQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _waveApiClient.GetBusinessesAsync(query.AccessToken, cancellationToken);
    }
}
