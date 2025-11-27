namespace Pharmacy.Application.Dtos.Authentication;
public record EmailVerificationDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
