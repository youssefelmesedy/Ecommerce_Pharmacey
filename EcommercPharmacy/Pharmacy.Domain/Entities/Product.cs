namespace Pharmacy.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; } = default;

        // One-to-Many: Product → ProductImages
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        // Order Items
        public ICollection<OrderItems>? OrderItems { get; set; }
    }
}
