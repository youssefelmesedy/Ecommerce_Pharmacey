namespace Pharmacy.Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = default!;

        public bool IsMain { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        // Foreign Key
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
