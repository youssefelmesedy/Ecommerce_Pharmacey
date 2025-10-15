namespace Pharmacy.Domain.Entities;
public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public decimal TotalAmount { get; set; } = 0;
    public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Shipped

    public string DeliveryAddress { get; set; } = default!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;


    // Navigation Property
    public ICollection<OrderItems>? OrderItems { get; set; }
}
