using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Stores Wave API credentials for a user.
/// Each user provides their own Wave Full Access Token.
/// </summary>
public class WaveUserCredential : BaseEntity
{
    /// <summary>
    /// The user ID (foreign key to ApplicationUser).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The encrypted Wave Full Access Token.
    /// </summary>
    public string EncryptedAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The selected Wave Business ID to sync.
    /// </summary>
    public string WaveBusinessId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the selected Wave business (for display).
    /// </summary>
    public string WaveBusinessName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the Wave connection is active.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// When the Wave account was connected (UTC).
    /// </summary>
    public DateTime? ConnectedAt { get; set; }

    /// <summary>
    /// When invoices were last successfully synced (UTC).
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Error message from the last sync attempt, if any.
    /// </summary>
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Records a successful sync operation.
    /// </summary>
    public void RecordSuccessfulSync()
    {
        LastSyncedAt = DateTime.UtcNow;
        LastSyncError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a sync error.
    /// </summary>
    /// <param name="error">The error message.</param>
    public void RecordSyncError(string error)
    {
        LastSyncError = error;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Connects to Wave with the provided credentials.
    /// </summary>
    /// <param name="encryptedToken">The encrypted access token.</param>
    /// <param name="businessId">The selected Wave business ID.</param>
    /// <param name="businessName">The selected Wave business name.</param>
    public void Connect(string encryptedToken, string businessId, string businessName)
    {
        EncryptedAccessToken = encryptedToken;
        WaveBusinessId = businessId;
        WaveBusinessName = businessName;
        IsConnected = true;
        ConnectedAt = DateTime.UtcNow;
        LastSyncError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Disconnects the Wave account.
    /// </summary>
    public void Disconnect()
    {
        EncryptedAccessToken = string.Empty;
        WaveBusinessId = string.Empty;
        WaveBusinessName = string.Empty;
        IsConnected = false;
        LastSyncError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the access token.
    /// </summary>
    /// <param name="encryptedToken">The new encrypted access token.</param>
    public void UpdateToken(string encryptedToken)
    {
        EncryptedAccessToken = encryptedToken;
        UpdatedAt = DateTime.UtcNow;
    }
}
