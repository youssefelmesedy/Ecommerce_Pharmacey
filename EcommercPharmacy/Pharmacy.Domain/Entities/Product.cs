namespace Pharmacy.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; } = default;

    // One-to-Many: Product → ProductImages
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // Order Items
    public ICollection<OrderItems>? OrderItems { get; set; }
}
