using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Dtos.Authentication;
using Pharmacy.Application.services.interfaces.authenticationinterface;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Enums;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService;
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserTokenService _userTokenService;

    public AuthenticationService(
        IUserService userService,
        IJwtTokenGenerator jwtTokenGenerator,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        ILogger<AuthenticationService> logger,
        IEmailService emailService,
        IUserTokenService userTokenService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _userTokenService = userTokenService ?? throw new ArgumentNullException(nameof(userTokenService));
    }

    #region 📝 RegisterAsync
    public async Task<AuthenticationDto> RegisterAsync(RegisterDto dto, string ipAddress, CancellationToken cancellationToken = default)
    {
        var auth = new AuthenticationDto();

        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        _logger.LogSection("REGISTER", $"📩 Registration attempt for: {dto.Email}");

        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Address = dto.Address,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                PhoneNumbers = dto.phoneNumbers.Select(p => new PhoneNumbers { phoneNumber = p }).ToList(),
                Role = "Customer",
            };

            var newUser = await _userService.CreateUserAsync(user, dto.ImageProfile, cancellationToken);
            if (newUser is null)
            {
                auth.Message = "There was a problem adding the user.";
                _logger.LogSection("REGISTER FAILED", "❌ User could not be created (returned null).", LogLevel.Warning);
                return auth;
            }

            var jwtAccessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(newUser, cancellationToken);
            if (string.IsNullOrWhiteSpace(jwtAccessToken.token))
            {
                auth.Message = "The Access Token is Empty";
                _logger.LogSection("TOKEN FAILED", "⚠️ Access token generation failed.", LogLevel.Warning);
                return auth;
            }

            var refreshToken = await _tokenService.CreateRefreshTokenAsync(newUser, ipAddress, cancellationToken);
            if (string.IsNullOrWhiteSpace(refreshToken.Token))
            {
                auth.Message = "Refresh token could not be generated.";
                _logger.LogSection("TOKEN FAILED", "⚠️ Refresh token generation failed.", LogLevel.Warning);
                return auth;
            }

            _logger.LogSection("REGISTER SUCCESS", $"✅ User '{newUser.Email}' registered successfully from IP: {ipAddress}");

            auth.IsActive = true;
            auth.FullName = newUser.FullName;
            auth.Email = newUser.Email;
            auth.AccessToken = jwtAccessToken.token;
            auth.ExpiresAccessToken = jwtAccessToken.expiresAtUtc;
            auth.RefreshToken = refreshToken.Token;
            auth.ExpiresRefreshToken = refreshToken.ExpiresAtUtc;

            return auth;
        }
        catch (Exception ex)
        {
            _logger.LogSection("REGISTER ERROR", $"❌ Unexpected error during registration for '{dto.Email}': {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region 🔐 LoginAsync
    public async Task<AuthenticationDto> LoginAsync(LoginDto dto, string ipAddress, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("LOGIN", $"🔐 Login attempt from IP: {ipAddress} | Email: {dto.Email}");

        try
        {
            var auth = new AuthenticationDto();

            var user = await _userService.GetUserByEmailAsync(dto.Email!, cancellationToken);
            if (user == null)
            {
                _logger.LogSection("LOGIN FAILED", $"⚠️ User not found: {dto.Email}", LogLevel.Warning);
                auth.Message = "Invalid email or password.";
                return auth;
            }

            if (!_passwordHasher.VerifyPassword(dto.Password!, user.PasswordHash))
            {
                _logger.LogSection("LOGIN FAILED", $"⚠️ Invalid password for email: {dto.Email}", LogLevel.Warning);
                auth.Message = "Invalid email or password.";
                return auth;
            }

            // ✅ تحقق من وجود RefreshToken صالح مسبقاً
            var existingRefreshToken = await _tokenService.GetValidRefreshTokenAsync(user.Id, cancellationToken);
            if (existingRefreshToken != null && existingRefreshToken!.IsActive)
            {
                _logger.LogSection("REFRESH TOKEN FOUND", $"♻️ Valid existing refresh token found for user '{user.Email}'");

                var (newAccessToken, expiresAt) = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, cancellationToken);
                auth.IsActive = true;
                auth.FullName = user.FullName;
                auth.Email = user.Email;
                auth.AccessToken = newAccessToken;
                auth.ExpiresAccessToken = expiresAt;
                auth.RefreshToken = existingRefreshToken.Token;
                auth.ExpiresRefreshToken = existingRefreshToken.ExpiresAtUtc;

                _logger.LogSection("LOGIN SUCCESS", $"✅ User '{user.Email}' reused existing refresh token successfully.");
                return auth;
            }

            // 🚫 لا يوجد RefreshToken صالح -> نلغي القديم (إن وجد)
            await _tokenService.RevokeOldTokensAsync(user.Id, ipAddress, "New Login", cancellationToken);
            _logger.LogSection("REFRESH TOKEN REVOKED", $"🧹 Old refresh tokens revoked for user '{user.Email}'");

            // 🎯 توليد Access Token جديد
            var (accessToken, expiresAtNew) = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogSection("TOKEN FAILED", "⚠️ Access token generation failed.", LogLevel.Warning);
                auth.Message = "Access token generation failed.";
                return auth;
            }

            // 🎯 توليد Refresh Token جديد
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user, ipAddress, cancellationToken);
            if (string.IsNullOrWhiteSpace(refreshToken.Token))
            {
                _logger.LogSection("TOKEN FAILED", "⚠️ Refresh token generation failed.", LogLevel.Warning);

                auth.Message = "Refresh token generation failed.";

                return auth;
            }

            _logger.LogSection("LOGIN SUCCESS", $"✅ User '{user.Email}' logged in successfully from {ipAddress}");

            auth.IsActive = true;
            auth.FullName = user.FullName;
            auth.Email = user.Email;
            auth.AccessToken = accessToken;
            auth.ExpiresAccessToken = expiresAtNew;
            auth.RefreshToken = refreshToken.Token;
            auth.ExpiresRefreshToken = refreshToken.ExpiresAtUtc;

            return auth;
        }
        catch (Exception ex)
        {
            _logger.LogSection("LOGIN ERROR", $"❌ Exception during login for '{dto.Email}': {ex.Message}", LogLevel.Error);
            throw; 
        }
    }
    #endregion

    #region ♻️ RefreshTokenAsync
    public async Task<RefreshTokenDto> RefreshTokenAsync(
        string token,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        _logger.LogSection("REFRESH TOKEN", $"♻️ Refresh attempt from IP: {ipAddress}");

        try
        {
            // ✅ 1. Validate refresh token
            _logger.LogSection("Validate refresh token", "🔍 Validating refresh token...");
            var (user, refreshTokenEntity) = await ValidateRefreshTokenAsync(token, cancellationToken);

            // ✅ 2. Generate new Access Token
            _logger.LogSection("Generate new Access Token", $"🔑 Generating new access token for '{user.Email}'...");
            var (newAccessToken, expiresAt) = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, cancellationToken);

            if (string.IsNullOrWhiteSpace(newAccessToken))
            {
                _logger.LogSection("TOKEN FAILED", "⚠️ Access token generation failed.", LogLevel.Warning);
                throw new SecurityTokenException("Failed to generate new access token");
            }

            // ✅ 3. Rotate the refresh token (revoke + create new)
            _logger.LogSection("Rotate the refresh token", "🔁 Rotate the refresh token (revoke + create new)");
            var newRefreshToken = await _tokenService.RotateRefreshTokenAsync(refreshTokenEntity, ipAddress, cancellationToken);

            if (string.IsNullOrWhiteSpace(newRefreshToken.Token))
            {
                _logger.LogSection("TOKEN FAILED", "⚠️ Refresh token rotation failed.", LogLevel.Warning);
                throw new SecurityTokenException("Failed to rotate refresh token");
            }


            // ✅ 5. Log success
            _logger.LogSection("REFRESH SUCCESS", $"✅ Tokens refreshed successfully for '{user.Email}'");
            return new RefreshTokenDto
            {
                AccessToken = newAccessToken,
                ExpiresAccessToken = expiresAt.ToString("yyyy-MM-dd, HH:mm:ss 'UTC'"),
                Token = newRefreshToken.Token,
                ExpiresAtUtc = newRefreshToken.ExpiresAtUtc,
                CreatedAtUtc = newRefreshToken.CreatedAtUtc.ToString("yyyy-MM-dd, HH:mm:ss 'UTC'"),
                CreatedByIp = newRefreshToken.CreatedByIp
            };
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogSection("REFRESH FAILED", $"⚠️ Invalid refresh token: {ex.Message}", LogLevel.Warning);
            throw;
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogSection("DATABASE ERROR", $"❌ DB update failed: {dbEx.InnerException?.Message ?? dbEx.Message}", LogLevel.Error);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogSection("REFRESH ERROR", $"❌ Unexpected error while refreshing token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region 🚪 LogoutAsync
    public async Task<string> LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("LOGOUT", $"🚪 Logout request from IP: {ipAddress}");

        try
        {
            var user = await _tokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (user != null)
            {
                await _tokenService.RevokeRefreshTokenAsync(user, ipAddress, "User logged out", cancellationToken);

                _logger.LogSection("LOGOUT SUCCESS", $"✅ User '{user.User.FullName}' logged out successfully.");

                return "Logut Successfully";
            }
            else
            {
                _logger.LogSection("LOGOUT FAILED", "⚠️ Refresh token invalid or user not found.", LogLevel.Warning);

                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogSection("LOGOUT ERROR", $"❌ Error during logout: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region EmailVerification
    #endregion

    #region 🔐 ResetPassword
    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, string ipAddress, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("RESET PASSWORD", $"🔁 Password reset requested for: {email}");

        try
        {
            // 1️⃣ تحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                _logger.LogSection("RESET PASSWORD ERROR", "Missing required fields (email/token/password).", LogLevel.Warning);
                return false;
            }

            // 2️⃣ جلب المستخدم
            var user = await _userService.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                _logger.LogSection("RESET PASSWORD ERROR", $"No user found with email: {email}", LogLevel.Warning);
                return false;
            }

            // 3️⃣ التحقق من صحة التوكن في قاعدة البيانات
            var userToken = await _userTokenService.GetUserTokenByToken(token, cancellationToken);
            if (userToken == null || userToken.IsUse || userToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogSection("RESET PASSWORD ERROR", $"Invalid or expired token for {email}.", LogLevel.Warning);
                return false;
            }

            // 4️⃣ تأكد أن التوكن فعلاً يخص نفس المستخدم
            if (userToken.UserId != user.Id)
            {
                _logger.LogSection("RESET PASSWORD ERROR", $"Token does not belong to user {email}.", LogLevel.Warning);
                return false;
            }


            // 5️⃣ تحديث كلمة المرور (تجزئة قبل الحفظ)
            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _logger.LogSection("RESET PASSWORD", $"✅ Password updated successfully for {email}.");

            // 6️⃣ إبطال التوكنات القديمة (اختياري لكن مستحسن)
            await _tokenService.RevokeOldTokensAsync(user.Id, ipAddress, "Reset Password", cancellationToken);
            _logger.LogSection("RESET PASSWORD", $"♻️ Old tokens revoked for {email}.");

            // 7️⃣ حفظ التغييرات
            await _userService.UpdateUserAsync(user, cancellationToken);
            _logger.LogSection("RESET PASSWORD", $"💾 Changes saved successfully for {email}.");

            userToken.IsUse = true;
            _logger.LogSection("RESET PASSWORD", $"Update User Token and Revoked Token userToken.IsUse = {userToken.IsUse}, {user.IsActive}.");
            await _userTokenService.UpdateUserTokenAsync(userToken, cancellationToken);

            return true;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogSection("RESET PASSWORD ERROR", $"⚠️ Invalid token encountered: {ex.Message}", LogLevel.Warning);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogSection("RESET PASSWORD ERROR", $"❌ Error resetting password for {email}: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region 🔁 ForgotPassword
    public async Task<bool> ForgotPasswordAsync(string email, string frontendResetUrl, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("FORGOT PASSWORD", $"🔍 Forgot password requested for: {email}");

        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogSection("FORGOT PASSWORD ERROR", "Email is empty.", LogLevel.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(frontendResetUrl))
            {
                _logger.LogSection("FORGOT PASSWORD ERROR", "Frontend reset URL is missing.", LogLevel.Warning);
                return false;
            }

            // 1️⃣ جلب المستخدم من قاعدة البيانات
            var user = await _userService.GetUserByEmailAsync(email, cancellationToken);

            // 🔒 خيار أمني: لا نخبر الـ frontend إذا كان البريد موجود أو لا
            if (user == null)
            {
                _logger.LogSection("FORGOT PASSWORD INFO", $"User with email {email} not found. Returning success to avoid email enumeration.");
                return true; // نحافظ على أمان النظام بعدم كشف وجود البريد
            }

            // 2️⃣ توليد توكن إعادة تعيين مؤقت
            var resetToken = await _jwtTokenGenerator.GeneratingShortTermTokens(user, "Forogt Password");

            //Add Generated Sort Term Tokens
            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = resetToken,
                CreateAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUse = false,
                TokenType = TokenType.ResetPassword,
            };
            await _userTokenService.CreateUserTokenAsync(userToken, cancellationToken);

            // 3️⃣ تكوين رابط إعادة التعيين للـ Frontend
            var encodedToken = Uri.EscapeDataString(resetToken);
            var encodedEmail = Uri.EscapeDataString(user.Email);

            // استخدام UriBuilder يجعل الرابط أكثر أمانًا وديناميكية
            var builder = new UriBuilder(frontendResetUrl);
            builder.Query = $"email={encodedEmail}&token={encodedToken}";
            var resetLink = builder.Uri.ToString();

            _logger.LogSection("FORGOT PASSWORD", $"🔗 Reset link generated for {email} (hidden for security).");

            // 4️⃣ إرسال البريد الإلكتروني للمستخدم
            await _emailService.SendPasswordResetAsync(user.Email, resetLink);
            _logger.LogSection("FORGOT PASSWORD", $"📧 Password reset email sent to {email}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogSection("FORGOT PASSWORD ERROR", $"❌ Error while processing forgot password for {email}: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    private async Task<(User, RefreshToken)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = await _tokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);

        if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            throw new SecurityTokenException("Invalid refresh token");

        var user = refreshTokenEntity.User;
        return (user, refreshTokenEntity);
    }
}
