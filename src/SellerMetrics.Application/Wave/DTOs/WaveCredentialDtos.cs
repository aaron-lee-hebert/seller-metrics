namespace SellerMetrics.Application.Wave.DTOs;

/// <summary>
/// DTO for Wave connection status.
/// </summary>
public class WaveConnectionStatusDto
{
    public bool IsConnected { get; set; }
    public string? BusinessName { get; set; }
    public string? BusinessId { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? LastSyncError { get; set; }
}
