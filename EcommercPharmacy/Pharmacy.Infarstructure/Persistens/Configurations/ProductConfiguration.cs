using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(p => p.SKU)
               .HasMaxLength(50);

        builder.Property(p => p.Price)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(p => p.DiscountPrice)
               .HasPrecision(18, 2);

        builder.Property(p => p.StockQuantity)
               .IsRequired();

        builder.Property(p => p.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(p => p.Name)
               .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => p.SKU)
               .IsUnique()
               .HasFilter("[SKU] IS NOT NULL")
               .HasDatabaseName("IX_Products_SKU");

        builder.HasIndex(p => p.CategoryId)
               .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.IsActive)
               .HasDatabaseName("IX_Products_IsActive");

        // Relationship with Category
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Images
        builder.HasMany(p => p.Images)
               .WithOne(i => i.Product)
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
