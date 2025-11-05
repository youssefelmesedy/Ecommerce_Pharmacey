namespace Pharmacy.Application.Dtos.Categories;
public record CategoryIncludeProductDto
{
    public Guid Category_Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ICollection<ProductWithCategoryDto> Products { get; init; } = new List<ProductWithCategoryDto>();
}
