using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for WaveInvoice entity.
/// </summary>
public class WaveInvoiceConfiguration : IEntityTypeConfiguration<WaveInvoice>
{
    public void Configure(EntityTypeBuilder<WaveInvoice> builder)
    {
        builder.ToTable("WaveInvoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(i => i.WaveInvoiceId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.CustomerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.WaveCustomerId)
            .HasMaxLength(100);

        builder.Property(i => i.Memo)
            .HasMaxLength(2000);

        builder.Property(i => i.ViewUrl)
            .HasMaxLength(500);

        // Money value objects
        builder.OwnsOne(i => i.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(i => i.AmountDue, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AmountDue")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("AmountDueCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(i => i.AmountPaid, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AmountPaid")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("AmountPaidCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Unique constraint on Wave invoice ID per user
        builder.HasIndex(i => new { i.UserId, i.WaveInvoiceId })
            .IsUnique();

        // Index for common queries
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.InvoiceDate);
        builder.HasIndex(i => i.Status);

        // Relationship to ApplicationUser
        builder.HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to payments
        builder.HasMany(i => i.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.WaveInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
