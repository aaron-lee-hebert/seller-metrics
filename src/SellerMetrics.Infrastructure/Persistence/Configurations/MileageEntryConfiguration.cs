using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the MileageEntry entity.
/// </summary>
public class MileageEntryConfiguration : IEntityTypeConfiguration<MileageEntry>
{
    public void Configure(EntityTypeBuilder<MileageEntry> builder)
    {
        builder.ToTable("MileageEntries");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TripDate)
            .IsRequired();

        builder.Property(m => m.Purpose)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.StartLocation)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Destination)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Miles)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(m => m.IsRoundTrip)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.BusinessLine)
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasMaxLength(2000);

        // Relationship with ServiceJob
        builder.HasOne(m => m.ServiceJob)
            .WithMany()
            .HasForeignKey(m => m.ServiceJobId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(m => !m.IsDeleted);

        // Indexes
        builder.HasIndex(m => m.TripDate);
        builder.HasIndex(m => m.BusinessLine);
        builder.HasIndex(m => m.ServiceJobId);
        builder.HasIndex(m => m.IsDeleted);
        builder.HasIndex(m => new { m.TripDate, m.BusinessLine });
    }
}
