using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the RevenueEntry entity.
/// </summary>
public class RevenueEntryConfiguration : IEntityTypeConfiguration<RevenueEntry>
{
    public void Configure(EntityTypeBuilder<RevenueEntry> builder)
    {
        builder.ToTable("RevenueEntries");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Source)
            .IsRequired();

        builder.Property(r => r.EntryType)
            .IsRequired();

        builder.Property(r => r.TransactionDate)
            .IsRequired();

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.EbayOrderId)
            .HasMaxLength(100);

        builder.Property(r => r.WaveInvoiceNumber)
            .HasMaxLength(100);

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        // Money value object - GrossAmount
        builder.OwnsOne(r => r.GrossAmount, grossBuilder =>
        {
            grossBuilder.Property(m => m.Amount)
                .HasColumnName("GrossAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            grossBuilder.Property(m => m.Currency)
                .HasColumnName("GrossCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Money value object - Fees
        builder.OwnsOne(r => r.Fees, feesBuilder =>
        {
            feesBuilder.Property(m => m.Amount)
                .HasColumnName("FeesAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            feesBuilder.Property(m => m.Currency)
                .HasColumnName("FeesCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Money value object - Shipping
        builder.OwnsOne(r => r.Shipping, shippingBuilder =>
        {
            shippingBuilder.Property(m => m.Amount)
                .HasColumnName("ShippingAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            shippingBuilder.Property(m => m.Currency)
                .HasColumnName("ShippingCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Money value object - TaxesCollected
        builder.OwnsOne(r => r.TaxesCollected, taxesBuilder =>
        {
            taxesBuilder.Property(m => m.Amount)
                .HasColumnName("TaxesCollectedAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            taxesBuilder.Property(m => m.Currency)
                .HasColumnName("TaxesCollectedCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Ensure owned entities are always initialized
        builder.Navigation(r => r.GrossAmount).IsRequired();
        builder.Navigation(r => r.Fees).IsRequired();
        builder.Navigation(r => r.Shipping).IsRequired();
        builder.Navigation(r => r.TaxesCollected).IsRequired();

        // Relationship with InventoryItem
        builder.HasOne(r => r.InventoryItem)
            .WithMany()
            .HasForeignKey(r => r.InventoryItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship with ServiceJob
        builder.HasOne(r => r.ServiceJob)
            .WithMany()
            .HasForeignKey(r => r.ServiceJobId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(r => !r.IsDeleted);

        // Indexes
        builder.HasIndex(r => r.Source);
        builder.HasIndex(r => r.EntryType);
        builder.HasIndex(r => r.TransactionDate);
        builder.HasIndex(r => r.EbayOrderId);
        builder.HasIndex(r => r.WaveInvoiceNumber);
        builder.HasIndex(r => r.InventoryItemId);
        builder.HasIndex(r => r.ServiceJobId);
        builder.HasIndex(r => r.IsDeleted);
        builder.HasIndex(r => new { r.Source, r.TransactionDate });
    }
}
