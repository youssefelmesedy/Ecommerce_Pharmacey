namespace Pharmacy.Application.Dtos.Authentication;

public class AuthenticationDto
{
    public bool IsActive { get; set; } = false;
    public string Message { get; set; } = "Empety";
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public DateTime ExpiresAccessToken { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresRefreshToken { get; set; }
}
