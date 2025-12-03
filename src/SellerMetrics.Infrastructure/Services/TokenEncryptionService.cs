using Microsoft.AspNetCore.DataProtection;
using SellerMetrics.Application.Ebay.Interfaces;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Service for encrypting and decrypting OAuth tokens using ASP.NET Core Data Protection.
/// </summary>
public class TokenEncryptionService : ITokenEncryptionService
{
    private readonly IDataProtector _protector;
    private const string Purpose = "SellerMetrics.EbayTokens.v1";

    public TokenEncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException("Plain text cannot be null or empty.", nameof(plainText));
        }

        return _protector.Protect(plainText);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            throw new ArgumentException("Cipher text cannot be null or empty.", nameof(cipherText));
        }

        return _protector.Unprotect(cipherText);
    }
}
