using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Dtos.Categories;
public record ProductWithCategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

}
