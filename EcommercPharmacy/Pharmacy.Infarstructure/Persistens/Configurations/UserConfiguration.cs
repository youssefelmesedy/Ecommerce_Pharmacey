using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.ProfileImageUrl)
               .HasMaxLength(500);

        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ✅ Indexes
        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.Role)
               .HasDatabaseName("IX_Users_Role");

        builder.HasIndex(u => u.FullName)
               .HasDatabaseName("IX_Users_FullName");

        builder.HasMany(u => u.PhoneNumbers)
               .WithOne(p => p.User)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Orders)
               .WithOne(o => o.User)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
