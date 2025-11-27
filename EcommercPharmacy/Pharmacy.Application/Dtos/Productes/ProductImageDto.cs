namespace Pharmacy.Application.Dtos.Productes;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}


