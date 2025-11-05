namespace Pharmacy.Application.services.interfaces.authenticationinterface;

public interface IEmailService
{
    Task SendEmailAsync(string toAddress, string subject, string body);
    Task SendEmailVerificationAsync(string email, string verificationCode);
    Task SendPasswordResetAsync(string email, string resetLink);
}