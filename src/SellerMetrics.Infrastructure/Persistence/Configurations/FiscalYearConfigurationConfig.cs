using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the FiscalYearConfiguration entity.
/// </summary>
public class FiscalYearConfigurationConfig : IEntityTypeConfiguration<FiscalYearConfiguration>
{
    public void Configure(EntityTypeBuilder<FiscalYearConfiguration> builder)
    {
        builder.ToTable("FiscalYearConfigurations");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FiscalYearStartMonth)
            .IsRequired();

        builder.Property(f => f.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Global query filter for soft delete
        builder.HasQueryFilter(f => !f.IsDeleted);

        // Index for finding active configuration
        builder.HasIndex(f => f.IsActive);

        // Seed default configuration (calendar year)
        builder.HasData(new FiscalYearConfiguration
        {
            Id = 1,
            FiscalYearStartMonth = 1,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
