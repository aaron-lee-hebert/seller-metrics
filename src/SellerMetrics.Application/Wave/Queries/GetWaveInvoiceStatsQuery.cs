using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Wave.Queries;

/// <summary>
/// Query to get Wave invoice statistics for a user.
/// </summary>
public record GetWaveInvoiceStatsQuery(
    string UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null);

/// <summary>
/// Handler for GetWaveInvoiceStatsQuery.
/// </summary>
public class GetWaveInvoiceStatsQueryHandler
{
    private readonly IWaveInvoiceRepository _invoiceRepository;

    public GetWaveInvoiceStatsQueryHandler(IWaveInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<WaveInvoiceStatsDto> HandleAsync(
        GetWaveInvoiceStatsQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = query.EndDate ?? DateTime.UtcNow;

        var invoices = await _invoiceRepository.GetByDateRangeAsync(
            query.UserId, startDate, endDate, cancellationToken);

        var paidInvoices = invoices.Where(i => i.Status == WaveInvoiceStatus.Paid).ToList();
        var unpaidInvoices = invoices.Where(i =>
            i.Status != WaveInvoiceStatus.Paid &&
            i.Status != WaveInvoiceStatus.Voided).ToList();
        var overdueInvoices = invoices.Where(i => i.IsOverdue).ToList();

        return new WaveInvoiceStatsDto
        {
            TotalInvoices = invoices.Count,
            PaidInvoices = paidInvoices.Count,
            UnpaidInvoices = unpaidInvoices.Count,
            OverdueInvoices = overdueInvoices.Count,
            TotalRevenue = paidInvoices.Sum(i => i.TotalAmount.Amount),
            TotalOutstanding = unpaidInvoices.Sum(i => i.AmountDue.Amount),
            Currency = "USD"
        };
    }
}
