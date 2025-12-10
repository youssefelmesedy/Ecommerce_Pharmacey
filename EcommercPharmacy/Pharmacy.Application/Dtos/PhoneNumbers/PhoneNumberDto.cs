namespace Pharmacy.Application.Dtos.PhoneNumbers;

public class PhoneNumberDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }
}
