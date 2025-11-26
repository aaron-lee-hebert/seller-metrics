using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the StorageLocation entity.
/// </summary>
public class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.ToTable("StorageLocations");

        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sl => sl.Description)
            .HasMaxLength(500);

        builder.Property(sl => sl.SortOrder)
            .HasDefaultValue(0);

        // Self-referencing relationship for hierarchy
        builder.HasOne(sl => sl.Parent)
            .WithMany(sl => sl.Children)
            .HasForeignKey(sl => sl.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(sl => !sl.IsDeleted);

        // Indexes
        builder.HasIndex(sl => sl.ParentId);
        builder.HasIndex(sl => sl.Name);
        builder.HasIndex(sl => sl.IsDeleted);
        builder.HasIndex(sl => new { sl.ParentId, sl.SortOrder });
    }
}
