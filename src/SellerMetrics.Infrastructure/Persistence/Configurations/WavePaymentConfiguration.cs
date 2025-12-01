using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for WavePayment entity.
/// </summary>
public class WavePaymentConfiguration : IEntityTypeConfiguration<WavePayment>
{
    public void Configure(EntityTypeBuilder<WavePayment> builder)
    {
        builder.ToTable("WavePayments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.WavePaymentId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        // Money value object
        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Unique constraint on Wave payment ID per user
        builder.HasIndex(p => new { p.UserId, p.WavePaymentId })
            .IsUnique();

        // Index for common queries
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.WaveInvoiceId);
        builder.HasIndex(p => p.PaymentDate);

        // Relationship to ApplicationUser
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
