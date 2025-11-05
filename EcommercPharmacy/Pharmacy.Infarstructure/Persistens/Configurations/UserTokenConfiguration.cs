using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;
public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(k => k.Id);

        builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.CreateAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.IsUse)
            .HasColumnName("IsUse");

        builder.HasOne(e => e.User)
            .WithMany(u => u.UserTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_UserToken_Token");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserToken_UserId");
    }
}
