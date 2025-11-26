using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the BusinessExpense entity.
/// </summary>
public class BusinessExpenseConfiguration : IEntityTypeConfiguration<BusinessExpense>
{
    public void Configure(EntityTypeBuilder<BusinessExpense> builder)
    {
        builder.ToTable("BusinessExpenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExpenseDate)
            .IsRequired();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Category)
            .IsRequired();

        builder.Property(e => e.BusinessLine)
            .IsRequired();

        builder.Property(e => e.Vendor)
            .HasMaxLength(200);

        builder.Property(e => e.ReceiptPath)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(e => e.IsTaxDeductible)
            .IsRequired()
            .HasDefaultValue(true);

        // Money value object - Amount
        builder.OwnsOne(e => e.Amount, amountBuilder =>
        {
            amountBuilder.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            amountBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Ensure owned entity is always initialized
        builder.Navigation(e => e.Amount).IsRequired();

        // Relationship with ServiceJob
        builder.HasOne(e => e.ServiceJob)
            .WithMany()
            .HasForeignKey(e => e.ServiceJobId)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Indexes
        builder.HasIndex(e => e.ExpenseDate);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.BusinessLine);
        builder.HasIndex(e => e.ServiceJobId);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => new { e.ExpenseDate, e.Category });
        builder.HasIndex(e => new { e.ExpenseDate, e.BusinessLine });
    }
}
