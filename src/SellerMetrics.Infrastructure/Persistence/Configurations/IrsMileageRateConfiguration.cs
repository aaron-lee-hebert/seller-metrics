using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the IrsMileageRate entity.
/// </summary>
public class IrsMileageRateConfiguration : IEntityTypeConfiguration<IrsMileageRate>
{
    public void Configure(EntityTypeBuilder<IrsMileageRate> builder)
    {
        builder.ToTable("IrsMileageRates");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Year)
            .IsRequired();

        builder.Property(r => r.StandardRate)
            .IsRequired()
            .HasColumnType("decimal(5,4)");

        builder.Property(r => r.MedicalRate)
            .HasColumnType("decimal(5,4)");

        builder.Property(r => r.CharitableRate)
            .HasColumnType("decimal(5,4)");

        builder.Property(r => r.EffectiveDate)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(500);

        // Global query filter for soft delete
        builder.HasQueryFilter(r => !r.IsDeleted);

        // Indexes
        builder.HasIndex(r => r.Year);
        builder.HasIndex(r => r.EffectiveDate);
        builder.HasIndex(r => new { r.Year, r.EffectiveDate });

        // Seed historical IRS mileage rates
        builder.HasData(
            new IrsMileageRate
            {
                Id = 1,
                Year = 2024,
                StandardRate = 0.67m,
                MedicalRate = 0.21m,
                CharitableRate = 0.14m,
                EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Notes = "IRS standard mileage rate for 2024",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new IrsMileageRate
            {
                Id = 2,
                Year = 2025,
                StandardRate = 0.70m,
                MedicalRate = 0.21m,
                CharitableRate = 0.14m,
                EffectiveDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Notes = "IRS standard mileage rate for 2025",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
