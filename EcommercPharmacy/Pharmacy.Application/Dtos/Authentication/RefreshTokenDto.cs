namespace Pharmacy.Application.Dtos.Authentication;
public class RefreshTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string ExpiresAccessToken { get; set; } = string.Empty;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string CreatedByIp { get; set; } = null!;
}
