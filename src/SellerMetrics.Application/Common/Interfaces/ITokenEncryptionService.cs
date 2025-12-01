namespace SellerMetrics.Application.Common.Interfaces;

/// <summary>
/// Interface for encrypting and decrypting sensitive tokens.
/// </summary>
public interface ITokenEncryptionService
{
    /// <summary>
    /// Encrypts a plain text token.
    /// </summary>
    /// <param name="plainText">The plain text token to encrypt.</param>
    /// <returns>The encrypted token as a base64 string.</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts an encrypted token.
    /// </summary>
    /// <param name="encryptedText">The encrypted token (base64 string).</param>
    /// <returns>The decrypted plain text token.</returns>
    string Decrypt(string encryptedText);
}
