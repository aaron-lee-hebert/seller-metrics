using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the InventoryItem entity.
/// </summary>
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InternalSku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.EbaySku)
            .HasMaxLength(50);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(4000);

        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.Property(i => i.PhotoPath)
            .HasMaxLength(500);

        builder.Property(i => i.EbayListingId)
            .HasMaxLength(50);

        // Money value object - owned entity
        builder.OwnsOne(i => i.Cogs, cogsBuilder =>
        {
            cogsBuilder.Property(m => m.Amount)
                .HasColumnName("CogsAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            cogsBuilder.Property(m => m.Currency)
                .HasColumnName("CogsCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Ensure Cogs is always initialized
        builder.Navigation(i => i.Cogs).IsRequired();

        // Relationship with StorageLocation
        builder.HasOne(i => i.StorageLocation)
            .WithMany()
            .HasForeignKey(i => i.StorageLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(i => !i.IsDeleted);

        // Indexes
        builder.HasIndex(i => i.InternalSku).IsUnique();
        builder.HasIndex(i => i.EbaySku);
        builder.HasIndex(i => i.EbayListingId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.StorageLocationId);
        builder.HasIndex(i => i.IsDeleted);
        builder.HasIndex(i => i.Title);
    }
}
