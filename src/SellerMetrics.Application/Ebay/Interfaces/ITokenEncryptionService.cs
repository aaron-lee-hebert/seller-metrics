namespace SellerMetrics.Application.Ebay.Interfaces;

/// <summary>
/// Service for encrypting and decrypting OAuth tokens.
/// </summary>
public interface ITokenEncryptionService
{
    /// <summary>
    /// Encrypts a plaintext token.
    /// </summary>
    /// <param name="plainText">The plaintext token.</param>
    /// <returns>The encrypted token.</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts an encrypted token.
    /// </summary>
    /// <param name="cipherText">The encrypted token.</param>
    /// <returns>The decrypted plaintext token.</returns>
    string Decrypt(string cipherText);
}
