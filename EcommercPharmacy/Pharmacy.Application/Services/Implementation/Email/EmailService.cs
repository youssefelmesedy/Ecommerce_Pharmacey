using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.services.interfaces.authenticationinterface;

namespace Pharmacy.Application.Services.Implementation.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// ترسل رسالة بريد إلكتروني عامة لأي غرض (تأكيد - إعادة تعيين كلمة مرور - إلخ)
        /// </summary>
        public async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            try
            {
                _logger.LogSection("EMAIL INIT", $"📧 Preparing to send email to: {toAddress}");

                // ✅ إنشاء رسالة جديدة
                var message = new MimeMessage();

                // 🧩 إضافة عنوان المرسل (من الإعدادات)
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SmtpUser));

                // 🧩 إضافة عنوان المستقبل
                message.To.Add(MailboxAddress.Parse(toAddress));

                // 🧩 تحديد عنوان الرسالة
                message.Subject = subject;

                // 🧩 تحديد جسم الرسالة (HTML أو نص)
                message.Body = new TextPart("html") { Text = body };

                _logger.LogSection("EMAIL MESSAGE", $"✅ Email composed successfully:\nSubject: {subject}");

                using var client = new SmtpClient();

                // ⚙️ الاتصال بخادم SMTP
                _logger.LogSection("SMTP CONNECT", $"Connecting to SMTP server: {_settings.SmtpServer}:{_settings.SmtpPort}");
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                // 🔐 المصادقة (Authentication)
                _logger.LogSection("SMTP AUTH", $"Authenticating user: {_settings.SmtpUser}");
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);

                // 📤 إرسال البريد الإلكتروني
                _logger.LogSection("SMTP SEND", $"Sending email to {toAddress}");
                await client.SendAsync(message);

                // 🔌 قطع الاتصال بالخادم بعد الإرسال
                await client.DisconnectAsync(true);

                _logger.LogSection("EMAIL SENT", $"✅ Email sent successfully to: {toAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogSection("EMAIL ERROR", $"❌ Failed to send email to {toAddress}\nError: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// ترسل رسالة تحقق من البريد الإلكتروني بعد التسجيل أو تحديث الإيميل
        /// </summary>
        public Task SendEmailVerificationAsync(string email, string verificationLink)
        {
            var subject = "Email Verification";
            var body = $"""
                <h3>Welcome!</h3>
                <p>Please verify your email by clicking the link below:</p>
                <p><a href="{verificationLink}" target="_blank">Verify Email</a></p>
            """;

            _logger.LogSection("EMAIL VERIFY", $"Preparing verification email for: {email}");
            return SendEmailAsync(email, subject, body);
        }

        /// <summary>
        /// ترسل رسالة لإعادة تعيين كلمة المرور تحتوي على رابط مخصص من الـ frontend
        /// </summary>
        public Task SendPasswordResetAsync(string email, string resetLink)
        {
            var subject = "Password Reset";
            var body = $"""
                <h3>Password Reset Request</h3>
                <p>Click the link below to reset your password:</p>
                <p><a href="{resetLink}" target="_blank">Reset Password</a></p>
                <p>If you didn't request a reset, please ignore this email.</p>
            """;

            _logger.LogSection("EMAIL RESET", $"Preparing password reset email for: {email}");
            return SendEmailAsync(email, subject, body);
        }
    }
}
