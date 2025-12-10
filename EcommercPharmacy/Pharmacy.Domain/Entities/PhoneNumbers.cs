namespace Pharmacy.Domain.Entities;

public class PhoneNumbers
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public bool IsVerified { get; set; } = false;

    // Foreign Key
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
