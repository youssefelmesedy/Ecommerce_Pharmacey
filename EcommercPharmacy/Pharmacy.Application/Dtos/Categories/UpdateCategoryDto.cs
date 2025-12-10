using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Categories;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
