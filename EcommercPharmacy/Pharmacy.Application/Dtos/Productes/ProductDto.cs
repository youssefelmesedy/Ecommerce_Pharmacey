namespace Pharmacy.Application.Dtos.Productes;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public decimal Price { get; set; }
    public decimal StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    // Category
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;

    // Images
    public List<ProductImageDto> Images { get; set; } = new();
}


