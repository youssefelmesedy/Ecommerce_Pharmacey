using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Products;

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? SKU { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? DiscountPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid CategoryId { get; set; }
}
