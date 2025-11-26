using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ComponentType entity.
/// </summary>
public class ComponentTypeConfiguration : IEntityTypeConfiguration<ComponentType>
{
    public void Configure(EntityTypeBuilder<ComponentType> builder)
    {
        builder.ToTable("ComponentTypes");

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ct => ct.Description)
            .HasMaxLength(500);

        builder.Property(ct => ct.DefaultExpenseCategory)
            .HasMaxLength(100);

        builder.Property(ct => ct.SortOrder)
            .HasDefaultValue(0);

        // Global query filter for soft delete
        builder.HasQueryFilter(ct => !ct.IsDeleted);

        // Indexes
        builder.HasIndex(ct => ct.Name).IsUnique();
        builder.HasIndex(ct => ct.IsDeleted);
        builder.HasIndex(ct => ct.SortOrder);

        // Seed predefined component types
        builder.HasData(
            new ComponentType { Id = 1, Name = "RAM", Description = "Random Access Memory modules", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 1 },
            new ComponentType { Id = 2, Name = "SSD", Description = "Solid State Drives", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 2 },
            new ComponentType { Id = 3, Name = "HDD", Description = "Hard Disk Drives", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 3 },
            new ComponentType { Id = 4, Name = "Power Supply", Description = "Power Supply Units (PSU)", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 4 },
            new ComponentType { Id = 5, Name = "CPU", Description = "Central Processing Units", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 5 },
            new ComponentType { Id = 6, Name = "Motherboard", Description = "System motherboards", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 6 },
            new ComponentType { Id = 7, Name = "Graphics Card", Description = "GPUs and video cards", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 7 },
            new ComponentType { Id = 8, Name = "Network Card", Description = "Network interface cards and adapters", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 8 },
            new ComponentType { Id = 9, Name = "Cooling", Description = "Fans, heatsinks, and cooling systems", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 9 },
            new ComponentType { Id = 10, Name = "Case", Description = "Computer cases and enclosures", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 10 },
            new ComponentType { Id = 11, Name = "Cables", Description = "Cables and connectors", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 11 },
            new ComponentType { Id = 12, Name = "Peripherals", Description = "Keyboards, mice, and other peripherals", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 12 },
            new ComponentType { Id = 13, Name = "Other", Description = "Other components", DefaultExpenseCategory = "Parts & Materials", IsSystemType = true, SortOrder = 99 }
        );
    }
}
