using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ComponentItem entity.
/// </summary>
public class ComponentItemConfiguration : IEntityTypeConfiguration<ComponentItem>
{
    public void Configure(EntityTypeBuilder<ComponentItem> builder)
    {
        builder.ToTable("ComponentItems");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.Property(c => c.LowStockThreshold)
            .IsRequired(false);

        // Money value object - owned entity
        builder.OwnsOne(c => c.UnitCost, costBuilder =>
        {
            costBuilder.Property(m => m.Amount)
                .HasColumnName("UnitCostAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            costBuilder.Property(m => m.Currency)
                .HasColumnName("UnitCostCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Ensure UnitCost is always initialized
        builder.Navigation(c => c.UnitCost).IsRequired();

        // Relationships
        builder.HasOne(c => c.ComponentType)
            .WithMany(ct => ct.Items)
            .HasForeignKey(c => c.ComponentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.StorageLocation)
            .WithMany()
            .HasForeignKey(c => c.StorageLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.ServiceJob)
            .WithMany(sj => sj.Components)
            .HasForeignKey(c => c.ServiceJobId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Indexes
        builder.HasIndex(c => c.ComponentTypeId);
        builder.HasIndex(c => c.StorageLocationId);
        builder.HasIndex(c => c.ServiceJobId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.Quantity);
    }
}
