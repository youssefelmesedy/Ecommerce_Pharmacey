using System.ComponentModel.DataAnnotations;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Dtos.Users;

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [EnumDataType(typeof(UserRole))]
    public UserRole Role { get; set; } = UserRole.Customer;
}
