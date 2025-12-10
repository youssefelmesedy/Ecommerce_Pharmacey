namespace Pharmacy.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = default!;
    public string? AltText { get; set; }

    public bool IsMain { get; set; } = false;

    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
}
