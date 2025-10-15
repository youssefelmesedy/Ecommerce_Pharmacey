namespace Pharmacy.Domain.Entities;
public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ProfileImageUrl { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";

    // Navigation Property
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<PhoneNumbers>? PhoneNumbers { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
