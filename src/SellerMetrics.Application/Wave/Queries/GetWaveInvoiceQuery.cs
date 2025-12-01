using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Queries;

/// <summary>
/// Query to get a specific Wave invoice.
/// </summary>
public record GetWaveInvoiceQuery(int InvoiceId, string UserId);

/// <summary>
/// Handler for GetWaveInvoiceQuery.
/// </summary>
public class GetWaveInvoiceQueryHandler
{
    private readonly IWaveInvoiceRepository _invoiceRepository;
    private readonly IWavePaymentRepository _paymentRepository;

    public GetWaveInvoiceQueryHandler(
        IWaveInvoiceRepository invoiceRepository,
        IWavePaymentRepository paymentRepository)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<WaveInvoiceDetailDto?> HandleAsync(
        GetWaveInvoiceQuery query,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(query.InvoiceId, cancellationToken);

        if (invoice == null || invoice.UserId != query.UserId)
            return null;

        var payments = await _paymentRepository.GetByInvoiceIdAsync(invoice.Id, cancellationToken);

        return new WaveInvoiceDetailDto
        {
            Id = invoice.Id,
            WaveInvoiceId = invoice.WaveInvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            WaveCustomerId = invoice.WaveCustomerId,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount.Amount,
            Currency = invoice.TotalAmount.Currency,
            AmountDue = invoice.AmountDue.Amount,
            AmountPaid = invoice.AmountPaid.Amount,
            Status = invoice.Status,
            Memo = invoice.Memo,
            ViewUrl = invoice.ViewUrl,
            IsOverdue = invoice.IsOverdue,
            LastSyncedAt = invoice.LastSyncedAt,
            Payments = payments.Select(p => new WavePaymentDetailDto
            {
                Id = p.Id,
                WavePaymentId = p.WavePaymentId,
                PaymentDate = p.PaymentDate,
                Amount = p.Amount.Amount,
                Currency = p.Amount.Currency,
                PaymentMethod = p.PaymentMethod,
                Notes = p.Notes
            }).ToList()
        };
    }
}
