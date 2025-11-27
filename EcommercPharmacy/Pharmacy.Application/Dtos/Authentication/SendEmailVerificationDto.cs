namespace Pharmacy.Application.Dtos.Authentication;

public record SendEmailVerificationDto
{
    public string Email { get; set; } = string.Empty;
    public string BaseURL { get; set; } = string.Empty;
}
