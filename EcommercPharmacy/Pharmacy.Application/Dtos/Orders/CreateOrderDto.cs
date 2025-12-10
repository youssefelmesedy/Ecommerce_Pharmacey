using System.ComponentModel.DataAnnotations;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Dtos.Orders;

public class CreateOrderDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod? PaymentMethod { get; set; }

    [Required]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}
