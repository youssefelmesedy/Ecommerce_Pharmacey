namespace Pharmacy.Application.Dtos.Productes;
public class CreateProductDto
{   
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
}
