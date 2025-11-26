using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ComponentQuantityAdjustment entity.
/// </summary>
public class ComponentQuantityAdjustmentConfiguration : IEntityTypeConfiguration<ComponentQuantityAdjustment>
{
    public void Configure(EntityTypeBuilder<ComponentQuantityAdjustment> builder)
    {
        builder.ToTable("ComponentQuantityAdjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.AdjustedAt)
            .IsRequired();

        // Relationship
        builder.HasOne(a => a.ComponentItem)
            .WithMany(c => c.Adjustments)
            .HasForeignKey(a => a.ComponentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.ComponentItemId);
        builder.HasIndex(a => a.AdjustedAt);
    }
}
