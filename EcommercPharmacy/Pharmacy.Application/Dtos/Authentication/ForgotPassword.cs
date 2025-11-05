namespace Pharmacy.Application.Dtos.Authentication;
public record  ForgotPassword
{
    public string? Email { get; set; }
    public string? BaseUrl { get; set; }
}
