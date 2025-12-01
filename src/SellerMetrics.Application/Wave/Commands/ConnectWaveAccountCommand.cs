using SellerMetrics.Application.Common.Interfaces;
using SellerMetrics.Application.Wave.Interfaces;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Commands;

/// <summary>
/// Command to connect a Wave account with an access token.
/// </summary>
public record ConnectWaveAccountCommand(
    string UserId,
    string AccessToken,
    string BusinessId,
    string BusinessName);

/// <summary>
/// Result of connecting a Wave account.
/// </summary>
public class ConnectWaveAccountResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Handler for ConnectWaveAccountCommand.
/// </summary>
public class ConnectWaveAccountCommandHandler
{
    private readonly IWaveUserCredentialRepository _credentialRepository;
    private readonly IWaveApiClient _waveApiClient;
    private readonly ITokenEncryptionService _tokenEncryptionService;
    private readonly IUnitOfWork _unitOfWork;

    public ConnectWaveAccountCommandHandler(
        IWaveUserCredentialRepository credentialRepository,
        IWaveApiClient waveApiClient,
        ITokenEncryptionService tokenEncryptionService,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _waveApiClient = waveApiClient;
        _tokenEncryptionService = tokenEncryptionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConnectWaveAccountResult> HandleAsync(
        ConnectWaveAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = new ConnectWaveAccountResult();

        try
        {
            // Validate the token first
            var isValid = await _waveApiClient.ValidateTokenAsync(command.AccessToken, cancellationToken);
            if (!isValid)
            {
                result.Error = "Invalid Wave access token. Please check your token and try again.";
                return result;
            }

            // Get or create credential record
            var credential = await _credentialRepository.GetByUserIdAsync(command.UserId, cancellationToken);

            if (credential == null)
            {
                credential = new WaveUserCredential
                {
                    UserId = command.UserId
                };
                await _credentialRepository.AddAsync(credential, cancellationToken);
            }

            // Connect with encrypted token
            var encryptedToken = _tokenEncryptionService.Encrypt(command.AccessToken);
            credential.Connect(encryptedToken, command.BusinessId, command.BusinessName);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Error = $"Failed to connect Wave account: {ex.Message}";
        }

        return result;
    }
}
