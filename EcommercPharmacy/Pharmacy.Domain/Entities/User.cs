using Pharmacy.Domain.Enums;

namespace Pharmacy.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;

    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? ProfileImageUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;

    // Navigation Property
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<UserToken>? UserTokens { get; set; }
    public ICollection<PhoneNumbers>? PhoneNumbers { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
