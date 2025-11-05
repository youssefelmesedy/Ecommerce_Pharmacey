using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Categories;
public class UpdateCategoryDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
