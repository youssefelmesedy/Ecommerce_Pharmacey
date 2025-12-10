using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(o => o.Status)
               .HasMaxLength(50)
               .HasDefaultValue(OrderStatus.Pending);

        builder.Property(o => o.DeliveryAddress)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(o => o.Notes)
               .HasMaxLength(1000);

        builder.Property(o => o.OrderDate)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.UpdatedAt)
               .IsRequired(false);

        builder.Property(o => o.CompletedAt)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(o => o.UserId)
               .HasDatabaseName("IX_Orders_UserId");

        builder.HasIndex(o => o.Status)
               .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(o => o.PaymentStatus)
               .HasDatabaseName("IX_Orders_PaymentStatus");

        builder.HasIndex(o => o.OrderDate)
               .HasDatabaseName("IX_Orders_OrderDate");

        // Relationship with User
        builder.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        // Relationship with OrderItems
        builder.HasMany(o => o.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
