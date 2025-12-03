namespace Pharmacy.Application.Dtos.Productes;
public record UpdateProductDto
{
    public Guid  ID { get; set; }
    public string? Name { get; set; } 
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
}
