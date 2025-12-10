using Pharmacy.Domain.Enums;

namespace Pharmacy.Domain.Entities;

public class UserToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public TokenType TokenType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    // Navigation Property
    public User? User { get; set; }
}
