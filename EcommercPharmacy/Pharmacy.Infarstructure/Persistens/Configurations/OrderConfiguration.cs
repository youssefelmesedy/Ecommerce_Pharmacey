using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
               .HasMaxLength(50)
               .HasDefaultValue(OrderStatus.Pending);

        builder.Property(o => o.DeliveryAddress)
               .IsRequired()
               .HasMaxLength(250);

        builder.Property(o => o.OrderDate)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(o => o.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        // ✅ Indexes
        builder.HasIndex(o => o.Status)
               .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(o => o.TotalAmount)
               .HasDatabaseName("IX_Orders_TotalAmount");

        builder.HasIndex(o => o.OrderDate)
               .HasDatabaseName("IX_Orders_OrderDate");
    }
}
