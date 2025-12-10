using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(ut => ut.Id);

        builder.Property(ut => ut.Token)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(ut => ut.TokenType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(ut => ut.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ut => ut.ExpiresAt)
               .IsRequired();

        builder.Property(ut => ut.IsUsed)
               .IsRequired()
               .HasDefaultValue(false);

        // Ignore computed property
        builder.Ignore(ut => ut.IsExpired);

        // Indexes
        builder.HasIndex(ut => ut.Token)
               .HasDatabaseName("IX_UserTokens_Token");

        builder.HasIndex(ut => ut.UserId)
               .HasDatabaseName("IX_UserTokens_UserId");

        builder.HasIndex(ut => ut.TokenType)
               .HasDatabaseName("IX_UserTokens_TokenType");

        // Relationship
        builder.HasOne(ut => ut.User)
               .WithMany(u => u.UserTokens)
               .HasForeignKey(ut => ut.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
