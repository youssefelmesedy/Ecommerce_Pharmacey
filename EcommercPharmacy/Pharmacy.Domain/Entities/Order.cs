using Pharmacy.Domain.Enums;

namespace Pharmacy.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public decimal TotalAmount { get; set; } = 0;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public PaymentMethod? PaymentMethod { get; set; }

    public string DeliveryAddress { get; set; } = default!;
    public string? Notes { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation Property
    public ICollection<OrderItems>? OrderItems { get; set; }
}
