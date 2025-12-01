using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Ebay.Commands;

/// <summary>
/// Command to disconnect an eBay account.
/// </summary>
public record DisconnectEbayAccountCommand(string UserId);

/// <summary>
/// Handler for DisconnectEbayAccountCommand.
/// </summary>
public class DisconnectEbayAccountCommandHandler
{
    private readonly IEbayUserCredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisconnectEbayAccountCommandHandler(
        IEbayUserCredentialRepository credentialRepository,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(
        DisconnectEbayAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (credential == null)
        {
            return false;
        }

        credential.Disconnect();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
