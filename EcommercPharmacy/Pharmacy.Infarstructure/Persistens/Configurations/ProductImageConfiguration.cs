using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ImageUrl)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(pi => pi.AltText)
               .HasMaxLength(200);

        builder.Property(pi => pi.IsMain)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(pi => pi.DisplayOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(pi => pi.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(pi => pi.ProductId)
               .HasDatabaseName("IX_ProductImages_ProductId");

        builder.HasIndex(pi => new { pi.ProductId, pi.IsMain })
               .HasDatabaseName("IX_ProductImages_ProductId_IsMain");

        // Relationship
        builder.HasOne(pi => pi.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
