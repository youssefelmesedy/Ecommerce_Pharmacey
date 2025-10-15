using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.ExpiresAtUtc)
                .IsRequired();

            builder.Property(e => e.CreatedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.RevokedAtUtc)
                .HasColumnName("RevokedAtUtc")
                .IsRequired(false); // ← مهم جداً

            builder.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(e => e.ExpiresAtUtc)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAtUtc");
        }
    }
}
