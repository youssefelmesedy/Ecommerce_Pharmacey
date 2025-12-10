using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Dtos.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderItemDto>? OrderItems { get; set; }
}
