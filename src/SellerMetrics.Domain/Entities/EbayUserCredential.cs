using SellerMetrics.Domain.Common;

namespace SellerMetrics.Domain.Entities;

/// <summary>
/// Stores eBay OAuth credentials for a user.
/// Tokens are stored encrypted using ASP.NET Core Data Protection.
/// </summary>
public class EbayUserCredential : BaseEntity
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
    /// The eBay user ID returned from authentication.
    /// </summary>
    public string? EbayUserId { get; set; }

    /// <summary>
    /// The eBay username for display purposes.
    /// </summary>
    public string? EbayUsername { get; set; }

    /// <summary>
    /// The encrypted OAuth access token.
    /// </summary>
    public string EncryptedAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The encrypted OAuth refresh token.
    /// </summary>
    public string EncryptedRefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// When the access token expires (UTC).
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// When the refresh token expires (UTC).
    /// Typically 18 months from issuance.
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// The OAuth scopes granted by the user.
    /// </summary>
    public string? Scopes { get; set; }

    /// <summary>
    /// The last time orders were successfully synced for this user (UTC).
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// The last sync error message, if any.
    /// Cleared on successful sync.
    /// </summary>
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Indicates whether the connection is currently active and usable.
    /// Set to false if the refresh token expires or user disconnects.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Returns true if the access token has expired.
    /// </summary>
    public bool IsAccessTokenExpired => DateTime.UtcNow >= AccessTokenExpiresAt;

    /// <summary>
    /// Returns true if the refresh token has expired.
    /// When this happens, user must re-authenticate.
    /// </summary>
    public bool IsRefreshTokenExpired => DateTime.UtcNow >= RefreshTokenExpiresAt;

    /// <summary>
    /// Updates the tokens after a successful refresh.
    /// </summary>
    /// <param name="encryptedAccessToken">The new encrypted access token.</param>
    /// <param name="accessTokenExpiresAt">When the new access token expires.</param>
    /// <param name="encryptedRefreshToken">The new encrypted refresh token (optional, some providers don't rotate).</param>
    /// <param name="refreshTokenExpiresAt">When the new refresh token expires (optional).</param>
    public void UpdateTokens(
        string encryptedAccessToken,
        DateTime accessTokenExpiresAt,
        string? encryptedRefreshToken = null,
        DateTime? refreshTokenExpiresAt = null)
    {
        EncryptedAccessToken = encryptedAccessToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;

        if (!string.IsNullOrEmpty(encryptedRefreshToken))
        {
            EncryptedRefreshToken = encryptedRefreshToken;
        }

        if (refreshTokenExpiresAt.HasValue)
        {
            RefreshTokenExpiresAt = refreshTokenExpiresAt.Value;
        }

        LastSyncError = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a successful sync.
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
    /// <param name="errorMessage">The error message.</param>
    public void RecordSyncError(string errorMessage)
    {
        LastSyncError = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Disconnects the eBay account.
    /// </summary>
    public void Disconnect()
    {
        IsConnected = false;
        EncryptedAccessToken = string.Empty;
        EncryptedRefreshToken = string.Empty;
        LastSyncError = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
