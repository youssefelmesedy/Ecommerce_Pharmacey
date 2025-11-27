namespace Pharmacy.Application.Dtos.Authentication;
public record  ForgotPasswordDto
{
    public string? Email { get; set; }
    public string? BaseUrl { get; set; }
}
