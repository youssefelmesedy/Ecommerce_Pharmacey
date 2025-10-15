namespace Pharmacy.Domain.Entities;
public class OrderItems
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public decimal Quantity { get; set; } = 0;
    public decimal UnitPrice { get; set; } = 0;

    // Totel = Quntity * Price
    public decimal Total  => Quantity * UnitPrice;
}
