using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerMetrics.Domain.Entities;

namespace SellerMetrics.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for WaveUserCredential entity.
/// </summary>
public class WaveUserCredentialConfiguration : IEntityTypeConfiguration<WaveUserCredential>
{
    public void Configure(EntityTypeBuilder<WaveUserCredential> builder)
    {
        builder.ToTable("WaveUserCredentials");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.EncryptedAccessToken)
            .IsRequired();

        builder.Property(c => c.WaveBusinessId)
            .HasMaxLength(100);

        builder.Property(c => c.WaveBusinessName)
            .HasMaxLength(255);

        builder.Property(c => c.LastSyncError)
            .HasMaxLength(2000);

        // One credential per user
        builder.HasIndex(c => c.UserId)
            .IsUnique();

        // Relationship to ApplicationUser
        builder.HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<WaveUserCredential>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
