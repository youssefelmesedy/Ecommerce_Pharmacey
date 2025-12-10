using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Orders;

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
