using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(150);

        builder.Property(p => p.Description)
               .HasMaxLength(500);

        builder.Property(p => p.Price)
               .HasColumnType("decimal(18,2)");

        builder.Property(p => p.StockQuantity)
               .HasColumnType("decimal(18,2)");

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.Images)
               .WithOne(i => i.Product)
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        // ✅ Indexes
        builder.HasIndex(p => p.Name)
               .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => p.Price)
               .HasDatabaseName("IX_Products_Price");

        builder.HasIndex(p => p.CreatedAt)
               .HasDatabaseName("IX_Products_CreatedAt");
    }
}
