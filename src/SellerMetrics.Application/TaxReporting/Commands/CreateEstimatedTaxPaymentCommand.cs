using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.TaxReporting.Commands;

/// <summary>
/// Command to create or update an estimated tax payment record.
/// </summary>
public record CreateEstimatedTaxPaymentCommand(
    int TaxYear,
    int Quarter,
    decimal EstimatedAmount,
    string Currency = "USD");

/// <summary>
/// Handler for CreateEstimatedTaxPaymentCommand.
/// </summary>
public class CreateEstimatedTaxPaymentCommandHandler
{
    private readonly IEstimatedTaxPaymentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEstimatedTaxPaymentCommandHandler(
        IEstimatedTaxPaymentRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(
        CreateEstimatedTaxPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Quarter < 1 || command.Quarter > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(command.Quarter), "Quarter must be between 1 and 4");
        }

        // Check if payment already exists for this quarter
        var existing = await _repository.GetByQuarterAsync(command.TaxYear, command.Quarter, cancellationToken);
        if (existing != null)
        {
            // Update existing record
            existing.EstimatedAmount = new Money(command.EstimatedAmount, command.Currency);
            existing.UpdatedAt = DateTime.UtcNow;
            _repository.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        // Create new payment record
        var payment = new EstimatedTaxPayment
        {
            TaxYear = command.TaxYear,
            Quarter = command.Quarter,
            DueDate = EstimatedTaxPayment.GetDueDate(command.TaxYear, command.Quarter),
            EstimatedAmount = new Money(command.EstimatedAmount, command.Currency),
            AmountPaid = Money.Zero(command.Currency),
            IsPaid = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return payment.Id;
    }
}
