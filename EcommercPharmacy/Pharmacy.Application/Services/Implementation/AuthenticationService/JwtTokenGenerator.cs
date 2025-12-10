using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Application.Settings;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JWTSetting _settings;
        private readonly byte[] _key;

        public JwtTokenGenerator(IOptions<JWTSetting> options)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(_settings.Key) || _settings.Key.Length < 16)
                throw new ArgumentException("JWT secret key must be at least 16 characters.");

            _key = Encoding.UTF8.GetBytes(_settings.Key);
        }

        // ✅ دالة توليد الـ Access Token العادي (زي ما كانت)
        public Task<(string token, DateTime expiresAtUtc)> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_settings.AccessTokenDurationInMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString() ?? "No't Role"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult((tokenStr, expires));
        }

        // ✅ توليد Password Reset Token قصير المدة
        public Task<string> GeneratingShortTermTokens(User user, string action)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(15); // صلاحية 15 دقيقة

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("Action", action), // Adding action claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        // ✅ التحقق من صلاحية Password Reset Token
        public bool ValidatePasswordResetToken(string token, string userEmail)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(token, parameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                var emailClaim = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
                var resetClaim = principal.FindFirst("ResetPasswordDto")?.Value;

                return emailClaim == userEmail && resetClaim == "true";
            }
            catch
            {
                return false;
            }
        }

        // ✅ موجودة أصلاً — لتحليل أي توكن آخر
        public ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrWhiteSpace(_settings.Issuer),
                ValidIssuer = _settings.Issuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(_settings.Audience),
                ValidAudience = _settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

                if (securityToken is JwtSecurityToken jwt &&
                    jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return principal;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
