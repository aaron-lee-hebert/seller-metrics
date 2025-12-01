using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Commands;

/// <summary>
/// Command to disconnect a Wave account.
/// </summary>
public record DisconnectWaveAccountCommand(string UserId);

/// <summary>
/// Result of disconnecting a Wave account.
/// </summary>
public class DisconnectWaveAccountResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Handler for DisconnectWaveAccountCommand.
/// </summary>
public class DisconnectWaveAccountCommandHandler
{
    private readonly IWaveUserCredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisconnectWaveAccountCommandHandler(
        IWaveUserCredentialRepository credentialRepository,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DisconnectWaveAccountResult> HandleAsync(
        DisconnectWaveAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = new DisconnectWaveAccountResult();

        try
        {
            var credential = await _credentialRepository.GetByUserIdAsync(command.UserId, cancellationToken);

            if (credential == null)
            {
                result.Error = "No Wave account connected.";
                return result;
            }

            credential.Disconnect();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Error = $"Failed to disconnect Wave account: {ex.Message}";
        }

        return result;
    }
}
