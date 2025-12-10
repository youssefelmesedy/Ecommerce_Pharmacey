using System.ComponentModel.DataAnnotations;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Dtos.Orders;

public class UpdateOrderDto
{
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus? Status { get; set; }

    [EnumDataType(typeof(PaymentStatus))]
    public PaymentStatus? PaymentStatus { get; set; }

    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod? PaymentMethod { get; set; }

    [StringLength(500)]
    public string? DeliveryAddress { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
