using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(rt => rt.CreatedByIp)
               .IsRequired()
               .HasMaxLength(45);

        builder.Property(rt => rt.RevokedByIp)
               .HasMaxLength(45);

        builder.Property(rt => rt.ReplacedByToken)
               .HasMaxLength(500);

        builder.Property(rt => rt.RevokedReason)
               .HasMaxLength(200);

        builder.Property(rt => rt.ExpiresAtUtc)
               .IsRequired();

        builder.Property(rt => rt.CreatedAtUtc)
               .HasDefaultValueSql("GETUTCDATE()");

        // Ignore computed properties
        builder.Ignore(rt => rt.IsActive);
        builder.Ignore(rt => rt.IsExpired);

        // Indexes
        builder.HasIndex(rt => rt.Token)
               .IsUnique()
               .HasDatabaseName("IX_RefreshTokens_Token");

        builder.HasIndex(rt => rt.UserId)
               .HasDatabaseName("IX_RefreshTokens_UserId");

        builder.HasIndex(rt => rt.ExpiresAtUtc)
               .HasDatabaseName("IX_RefreshTokens_ExpiresAtUtc");

        // Relationship
        builder.HasOne(rt => rt.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
