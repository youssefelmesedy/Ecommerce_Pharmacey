using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItems>
{
    public void Configure(EntityTypeBuilder<OrderItems> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
               .IsRequired();

        builder.Property(oi => oi.UnitPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(oi => oi.ProductName)
               .HasMaxLength(200);

        // Ignore computed property
        builder.Ignore(oi => oi.Total);

        // Indexes
        builder.HasIndex(oi => oi.OrderId)
               .HasDatabaseName("IX_OrderItems_OrderId");

        builder.HasIndex(oi => oi.ProductId)
               .HasDatabaseName("IX_OrderItems_ProductId");

        // Relationship with Order
        builder.HasOne(oi => oi.Order)
               .WithMany(o => o.OrderItems)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Product
        builder.HasOne(oi => oi.Product)
               .WithMany(p => p.OrderItems)
               .HasForeignKey(oi => oi.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
