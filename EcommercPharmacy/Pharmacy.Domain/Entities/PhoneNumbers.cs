namespace Pharmacy.Domain.Entities;
public class PhoneNumbers 
{
    public Guid Id { get; set; }
    public string phoneNumber { get; set; }
    public bool IsPrimary { get; set; }

    // Foreign Key
    public Guid UserId { get; set; }
    public User? User { get; set; }

}
