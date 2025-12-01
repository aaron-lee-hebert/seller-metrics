using SellerMetrics.Application.Common.Interfaces;
using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Application.Wave.Interfaces;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Wave.Commands;

/// <summary>
/// Command to sync invoices from Wave for a specific user.
/// </summary>
public record SyncInvoicesFromWaveCommand(
    string UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null);

/// <summary>
/// Result of syncing invoices from Wave.
/// </summary>
public class SyncInvoicesResult
{
    public int InvoicesSynced { get; set; }
    public int InvoicesCreated { get; set; }
    public int InvoicesUpdated { get; set; }
    public int InvoicesSkipped { get; set; }
    public int PaymentsSynced { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool Success => Errors.Count == 0;
}

/// <summary>
/// Handler for SyncInvoicesFromWaveCommand.
/// </summary>
public class SyncInvoicesFromWaveCommandHandler
{
    private readonly IWaveApiClient _waveApiClient;
    private readonly IWaveUserCredentialRepository _credentialRepository;
    private readonly IWaveInvoiceRepository _invoiceRepository;
    private readonly IWavePaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public SyncInvoicesFromWaveCommandHandler(
        IWaveApiClient waveApiClient,
        IWaveUserCredentialRepository credentialRepository,
        IWaveInvoiceRepository invoiceRepository,
        IWavePaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        ITokenEncryptionService tokenEncryptionService)
    {
        _waveApiClient = waveApiClient;
        _credentialRepository = credentialRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _tokenEncryptionService = tokenEncryptionService;
    }

    public async Task<SyncInvoicesResult> HandleAsync(
        SyncInvoicesFromWaveCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = new SyncInvoicesResult();

        // Get user credentials
        var credential = await _credentialRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (credential == null || !credential.IsConnected)
        {
            result.Errors.Add("Wave account is not connected.");
            return result;
        }

        try
        {
            // Decrypt the access token
            var accessToken = _tokenEncryptionService.Decrypt(credential.EncryptedAccessToken);

            // Default to last 30 days if no date range specified
            var startDate = command.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = command.EndDate ?? DateTime.UtcNow;

            // Fetch invoices from Wave
            var waveInvoices = await _waveApiClient.GetInvoicesAsync(
                accessToken,
                credential.WaveBusinessId,
                startDate,
                endDate,
                cancellationToken);

            foreach (var waveInvoice in waveInvoices)
            {
                try
                {
                    await ProcessInvoiceAsync(waveInvoice, command.UserId, result, cancellationToken);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error processing invoice {waveInvoice.InvoiceNumber}: {ex.Message}");
                }
            }

            // Record successful sync
            credential.RecordSuccessfulSync();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.InvoicesSynced = waveInvoices.Count;
        }
        catch (Exception ex)
        {
            credential.RecordSyncError(ex.Message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            result.Errors.Add($"Sync failed: {ex.Message}");
        }

        return result;
    }

    private async Task ProcessInvoiceAsync(
        WaveInvoiceDto waveInvoice,
        string userId,
        SyncInvoicesResult result,
        CancellationToken cancellationToken)
    {
        // Check if invoice already exists
        var existingInvoice = await _invoiceRepository.GetByWaveInvoiceIdAsync(
            waveInvoice.Id, userId, cancellationToken);

        if (existingInvoice != null)
        {
            // Update existing invoice
            UpdateExistingInvoice(existingInvoice, waveInvoice);
            result.InvoicesUpdated++;

            // Sync payments for existing invoice
            await SyncPaymentsAsync(existingInvoice, waveInvoice.Payments, userId, result, cancellationToken);
        }
        else
        {
            // Check if invoice was previously deleted - skip if so
            var wasDeleted = await _invoiceRepository.WasDeletedAsync(waveInvoice.Id, userId, cancellationToken);
            if (wasDeleted)
            {
                result.InvoicesSkipped++;
                return;
            }

            // Create new invoice
            var newInvoice = CreateNewInvoice(waveInvoice, userId);
            await _invoiceRepository.AddAsync(newInvoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.InvoicesCreated++;

            // Sync payments for new invoice
            await SyncPaymentsAsync(newInvoice, waveInvoice.Payments, userId, result, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void UpdateExistingInvoice(WaveInvoice invoice, WaveInvoiceDto waveInvoice)
    {
        invoice.UpdateFromSync(
            ParseInvoiceStatus(waveInvoice.Status),
            new Money(waveInvoice.Total.Value, waveInvoice.Total.Currency),
            new Money(waveInvoice.AmountDue.Value, waveInvoice.AmountDue.Currency),
            new Money(waveInvoice.AmountPaid.Value, waveInvoice.AmountPaid.Currency),
            waveInvoice.Memo,
            waveInvoice.ViewUrl);
    }

    private WaveInvoice CreateNewInvoice(WaveInvoiceDto waveInvoice, string userId)
    {
        return new WaveInvoice
        {
            UserId = userId,
            WaveInvoiceId = waveInvoice.Id,
            InvoiceNumber = waveInvoice.InvoiceNumber,
            CustomerName = waveInvoice.Customer?.Name ?? "Unknown",
            WaveCustomerId = waveInvoice.Customer?.Id,
            InvoiceDate = waveInvoice.InvoiceDate,
            DueDate = waveInvoice.DueDate,
            TotalAmount = new Money(waveInvoice.Total.Value, waveInvoice.Total.Currency),
            AmountDue = new Money(waveInvoice.AmountDue.Value, waveInvoice.AmountDue.Currency),
            AmountPaid = new Money(waveInvoice.AmountPaid.Value, waveInvoice.AmountPaid.Currency),
            Status = ParseInvoiceStatus(waveInvoice.Status),
            Memo = waveInvoice.Memo,
            ViewUrl = waveInvoice.ViewUrl,
            LastSyncedAt = DateTime.UtcNow
        };
    }

    private async Task SyncPaymentsAsync(
        WaveInvoice invoice,
        List<WavePaymentDto> wavePayments,
        string userId,
        SyncInvoicesResult result,
        CancellationToken cancellationToken)
    {
        foreach (var wavePayment in wavePayments)
        {
            var existingPayment = await _paymentRepository.GetByWavePaymentIdAsync(
                wavePayment.Id, userId, cancellationToken);

            if (existingPayment != null)
            {
                // Update existing payment
                existingPayment.UpdateFromSync(
                    new Money(wavePayment.Amount.Value, wavePayment.Amount.Currency),
                    wavePayment.PaymentMethod,
                    wavePayment.Notes);
            }
            else
            {
                // Create new payment
                var newPayment = new WavePayment
                {
                    UserId = userId,
                    WavePaymentId = wavePayment.Id,
                    WaveInvoiceId = invoice.Id,
                    PaymentDate = wavePayment.PaymentDate,
                    Amount = new Money(wavePayment.Amount.Value, wavePayment.Amount.Currency),
                    PaymentMethod = wavePayment.PaymentMethod,
                    Notes = wavePayment.Notes,
                    LastSyncedAt = DateTime.UtcNow
                };
                await _paymentRepository.AddAsync(newPayment, cancellationToken);
                result.PaymentsSynced++;
            }
        }
    }

    private static WaveInvoiceStatus ParseInvoiceStatus(string status) => status.ToUpperInvariant() switch
    {
        "DRAFT" => WaveInvoiceStatus.Draft,
        "SENT" => WaveInvoiceStatus.Sent,
        "VIEWED" => WaveInvoiceStatus.Viewed,
        "PARTIAL" or "PARTIALLY_PAID" => WaveInvoiceStatus.PartiallyPaid,
        "PAID" => WaveInvoiceStatus.Paid,
        "OVERDUE" => WaveInvoiceStatus.Overdue,
        "VOIDED" or "VOID" => WaveInvoiceStatus.Voided,
        _ => WaveInvoiceStatus.Draft
    };
}
