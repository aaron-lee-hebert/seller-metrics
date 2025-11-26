using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for EstimatedTaxPayment entity.
/// </summary>
public class EstimatedTaxPaymentConfiguration : IEntityTypeConfiguration<EstimatedTaxPayment>
{
    public void Configure(EntityTypeBuilder<EstimatedTaxPayment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TaxYear)
            .IsRequired();

        builder.Property(e => e.Quarter)
            .IsRequired();

        builder.Property(e => e.DueDate)
            .IsRequired();

        // Configure Money value objects as owned entities
        builder.OwnsOne(e => e.EstimatedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("EstimatedAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("EstimatedCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(e => e.AmountPaid, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AmountPaid")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("PaidCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(e => e.IsPaid)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ConfirmationNumber)
            .HasMaxLength(100);

        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Unique constraint on TaxYear + Quarter
        builder.HasIndex(e => new { e.TaxYear, e.Quarter })
            .IsUnique();

        // Index for finding overdue payments
        builder.HasIndex(e => new { e.IsPaid, e.DueDate });

        // Ignore computed properties
        builder.Ignore(e => e.QuarterDisplay);
        builder.Ignore(e => e.IsOverdue);
        builder.Ignore(e => e.RemainingAmount);
    }
}
