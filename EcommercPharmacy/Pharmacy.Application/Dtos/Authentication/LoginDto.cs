namespace Pharmacy.Application.Dtos.Authentication;
public record LoginDto 
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
