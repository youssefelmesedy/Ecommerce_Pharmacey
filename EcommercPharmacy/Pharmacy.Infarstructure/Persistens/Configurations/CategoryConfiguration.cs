using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .HasMaxLength(500);

        builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Index
        builder.HasIndex(c => c.Name)
               .HasDatabaseName("IX_Categories_Name");

        builder.HasIndex(c => c.IsActive)
               .HasDatabaseName("IX_Categories_IsActive");

        builder.HasMany(c => c.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
