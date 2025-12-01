using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Queries;

/// <summary>
/// Query to get a list of Wave invoices for a user.
/// </summary>
public record GetWaveInvoiceListQuery(
    string UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    WaveInvoiceStatus? Status = null);

/// <summary>
/// Handler for GetWaveInvoiceListQuery.
/// </summary>
public class GetWaveInvoiceListQueryHandler
{
    private readonly IWaveInvoiceRepository _invoiceRepository;

    public GetWaveInvoiceListQueryHandler(IWaveInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IReadOnlyList<WaveInvoiceListItemDto>> HandleAsync(
        GetWaveInvoiceListQuery query,
        CancellationToken cancellationToken = default)
    {
        var invoices = query switch
        {
            { Status: not null } => await _invoiceRepository.GetByStatusAsync(
                query.UserId, query.Status.Value, cancellationToken),
            { StartDate: not null, EndDate: not null } => await _invoiceRepository.GetByDateRangeAsync(
                query.UserId, query.StartDate.Value, query.EndDate.Value, cancellationToken),
            _ => await _invoiceRepository.GetByUserIdAsync(query.UserId, cancellationToken)
        };

        return invoices.Select(i => new WaveInvoiceListItemDto
        {
            Id = i.Id,
            WaveInvoiceId = i.WaveInvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            TotalAmount = i.TotalAmount.Amount,
            Currency = i.TotalAmount.Currency,
            AmountDue = i.AmountDue.Amount,
            AmountPaid = i.AmountPaid.Amount,
            Status = i.Status,
            IsOverdue = i.IsOverdue,
            ViewUrl = i.ViewUrl
        }).ToList();
    }
}
