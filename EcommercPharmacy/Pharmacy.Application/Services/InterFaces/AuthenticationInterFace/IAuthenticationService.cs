using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

public interface IAuthenticationService
{
    Task<AuthenticationDto> RegisterAsync(RegisterDto dto, string ipAddress, CancellationToken cancellationToken = default);

    Task<AuthenticationDto> LoginAsync(LoginDto dto, string ipAddress, CancellationToken cancellationToken = default);

    Task<RefreshTokenDto> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default);

    Task<string> LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);

    Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);

    Task<bool> ResetPasswordAsync(ResetPasswordDto dto, string ipAddress, CancellationToken cancellationToken = default);

    Task<(string, bool)> SendEmailVerificationAsync(SendEmailVerificationDto dto, CancellationToken cancellationToken = default);

    Task<bool> EmailVerificationAsync(EmailVerificationDto dto, CancellationToken cancellationToken = default);
}
