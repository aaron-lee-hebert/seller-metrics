using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the EbayOrder entity.
/// </summary>
public class EbayOrderConfiguration : IEntityTypeConfiguration<EbayOrder>
{
    public void Configure(EntityTypeBuilder<EbayOrder> builder)
    {
        builder.ToTable("EbayOrders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(o => o.EbayOrderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.LegacyOrderId)
            .HasMaxLength(100);

        builder.Property(o => o.BuyerUsername)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ItemTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.EbayItemId)
            .HasMaxLength(50);

        builder.Property(o => o.Sku)
            .HasMaxLength(50);

        builder.Property(o => o.Notes)
            .HasMaxLength(2000);

        // Money value objects - owned entities
        builder.OwnsOne(o => o.GrossSale, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("GrossSaleAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("GrossSaleCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.GrossSale).IsRequired();

        builder.OwnsOne(o => o.ShippingPaid, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("ShippingPaidAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("ShippingPaidCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.ShippingPaid).IsRequired();

        builder.OwnsOne(o => o.ShippingActual, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("ShippingActualAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("ShippingActualCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.ShippingActual).IsRequired();

        builder.OwnsOne(o => o.FinalValueFee, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("FinalValueFeeAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("FinalValueFeeCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.FinalValueFee).IsRequired();

        builder.OwnsOne(o => o.PaymentProcessingFee, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("PaymentProcessingFeeAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("PaymentProcessingFeeCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.PaymentProcessingFee).IsRequired();

        builder.OwnsOne(o => o.AdditionalFees, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("AdditionalFeesAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("AdditionalFeesCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
        builder.Navigation(o => o.AdditionalFees).IsRequired();

        // Relationships
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.InventoryItem)
            .WithMany()
            .HasForeignKey(o => o.InventoryItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(o => !o.IsDeleted);

        // Indexes
        builder.HasIndex(o => new { o.UserId, o.EbayOrderId }).IsUnique();
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.InventoryItemId);
        builder.HasIndex(o => o.IsDeleted);
    }
}
