using Microsoft.AspNetCore.DataProtection;
using SellerMetrics.Application.Common.Interfaces;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Token encryption service using ASP.NET Core Data Protection.
/// </summary>
public class TokenEncryptionService : ITokenEncryptionService
{
    private readonly IDataProtector _protector;
    private const string Purpose = "SellerMetrics.ApiTokens";

    public TokenEncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        return _protector.Protect(plainText);
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        return _protector.Unprotect(encryptedText);
    }
}
