using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl)
               .IsRequired()
               .HasMaxLength(500) ;

        builder.Property(i => i.IsMain)
               .HasDefaultValue(false);

        builder.Property(i => i.DisplayOrder)
               .HasDefaultValue(0);

        builder.HasOne(i => i.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        // ✅ Index to sort and filter quickly
        builder.HasIndex(i => new { i.ProductId, i.DisplayOrder })
               .HasDatabaseName("IX_ProductImages_Product_DisplayOrder");
    }
}
