using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ServiceJob entity.
/// </summary>
public class ServiceJobConfiguration : IEntityTypeConfiguration<ServiceJob>
{
    public void Configure(EntityTypeBuilder<ServiceJob> builder)
    {
        builder.ToTable("ServiceJobs");

        builder.HasKey(sj => sj.Id);

        builder.Property(sj => sj.JobNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sj => sj.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sj => sj.CustomerContact)
            .HasMaxLength(200);

        builder.Property(sj => sj.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sj => sj.Notes)
            .HasMaxLength(4000);

        builder.Property(sj => sj.WaveInvoiceId)
            .HasMaxLength(50);

        // Global query filter for soft delete
        builder.HasQueryFilter(sj => !sj.IsDeleted);

        // Indexes
        builder.HasIndex(sj => sj.JobNumber).IsUnique();
        builder.HasIndex(sj => sj.CustomerName);
        builder.HasIndex(sj => sj.Status);
        builder.HasIndex(sj => sj.ReceivedDate);
        builder.HasIndex(sj => sj.IsDeleted);
        builder.HasIndex(sj => sj.WaveInvoiceId);
    }
}
