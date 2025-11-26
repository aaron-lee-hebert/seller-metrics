using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.TaxReporting.Commands;

/// <summary>
/// Command to record an estimated tax payment as paid.
/// </summary>
public record RecordTaxPaymentCommand(
    int TaxYear,
    int Quarter,
    decimal AmountPaid,
    DateTime PaidDate,
    string? ConfirmationNumber = null,
    string? PaymentMethod = null,
    string? Notes = null,
    string Currency = "USD");

/// <summary>
/// Handler for RecordTaxPaymentCommand.
/// </summary>
public class RecordTaxPaymentCommandHandler
{
    private readonly IEstimatedTaxPaymentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordTaxPaymentCommandHandler(
        IEstimatedTaxPaymentRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(
        RecordTaxPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetByQuarterAsync(command.TaxYear, command.Quarter, cancellationToken);
        if (payment == null)
        {
            return false;
        }

        payment.AmountPaid = new Money(command.AmountPaid, command.Currency);
        payment.PaidDate = command.PaidDate;
        payment.IsPaid = command.AmountPaid >= payment.EstimatedAmount.Amount;
        payment.ConfirmationNumber = command.ConfirmationNumber;
        payment.PaymentMethod = command.PaymentMethod;
        payment.Notes = command.Notes;
        payment.UpdatedAt = DateTime.UtcNow;

        _repository.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
