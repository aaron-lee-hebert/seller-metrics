using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the EbayUserCredential entity.
/// </summary>
public class EbayUserCredentialConfiguration : IEntityTypeConfiguration<EbayUserCredential>
{
    public void Configure(EntityTypeBuilder<EbayUserCredential> builder)
    {
        builder.ToTable("EbayUserCredentials");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(450); // Match Identity user ID length

        builder.Property(c => c.EbayUserId)
            .HasMaxLength(100);

        builder.Property(c => c.EbayUsername)
            .HasMaxLength(100);

        builder.Property(c => c.EncryptedAccessToken)
            .IsRequired()
            .HasMaxLength(4000); // Encrypted tokens can be long

        builder.Property(c => c.EncryptedRefreshToken)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.Scopes)
            .HasMaxLength(2000);

        builder.Property(c => c.LastSyncError)
            .HasMaxLength(2000);

        // Relationship with ApplicationUser
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Indexes
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.IsConnected);
        builder.HasIndex(c => c.IsDeleted);
    }
}
