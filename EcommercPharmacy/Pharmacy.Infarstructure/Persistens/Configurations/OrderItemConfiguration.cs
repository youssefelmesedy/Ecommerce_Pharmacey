using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItems>
{
    public void Configure(EntityTypeBuilder<OrderItems> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
               .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.UnitPrice)
               .HasColumnType("decimal(18,2)");

        builder.HasOne(oi => oi.Product)
               .WithMany(p => p.OrderItems)
               .HasForeignKey(oi => oi.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        // ✅ Composite index (Order + Product)
        builder.HasIndex(oi => new { oi.OrderId, oi.ProductId })
               .HasDatabaseName("IX_OrderItems_Order_Product");
    }
}
