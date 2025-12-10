namespace Pharmacy.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string CreatedByIp { get; set; } = null!;
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? RevokedReason { get; set; }

    public bool IsActive => RevokedAtUtc == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    // Foreign Key
    public Guid UserId { get; set; }
    // Navigation Property
    public User User { get; set; } = null!;
}
