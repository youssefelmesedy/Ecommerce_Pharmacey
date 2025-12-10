using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.PhoneNumbers;

public class CreatePhoneNumberDto
{
    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;
}
