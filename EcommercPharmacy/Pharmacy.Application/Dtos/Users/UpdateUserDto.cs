using System.ComponentModel.DataAnnotations;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Dtos.Users;

public class UpdateUserDto
{
    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(500)]
    public string? ProfileImageUrl { get; set; }

    [EnumDataType(typeof(UserRole))]
    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}
