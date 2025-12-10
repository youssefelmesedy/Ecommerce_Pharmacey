using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Categories;

public class CreateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
