using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Application.Settings;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using System.Security.Cryptography;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JWTSetting _jwtSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IUnitOfWork unitOfWork, IOptions<JWTSetting> jwtOptions, ILogger<TokenService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtSettings = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(User user, string createdByIp, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("CREATE REFRESH TOKEN", $"🪙 Creating refresh token for user: {user.Email}");

        try
        {
            var token = GenerateSecureToken(64);
            var now = DateTime.UtcNow;

            var refresh = new RefreshToken
            {
                Token = token,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                CreatedByIp = createdByIp,
                UserId = user.Id
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refresh, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogSection("TOKEN CREATED", $"✅ Refresh token created successfully for {user.Email}");
            return refresh;
        }
        catch (Exception ex)
        {
            _logger.LogSection("TOKEN ERROR", $"❌ Failed to create refresh token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken token, string revokedByIp, string reason = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!token.IsActive)
            {
                _logger.LogSection("TOKEN REVOKE", $"⚠️ Token already inactive for user {token.User?.Email}");
                return;
            }

            token.RevokedAtUtc = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
            token.RevokedReason = reason;

            await _unitOfWork.Repository<RefreshToken>().Update(token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogSection("TOKEN REVOKE", $"🚫 Token revoked for user {token.User?.Email} - Reason: {reason}");
        }
        catch (Exception ex)
        {
            _logger.LogSection("TOKEN REVOKE ERROR", $"❌ Failed to revoke token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task RevokeOldTokensAsync(Guid userId, string ipAddress, string reason = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryOptions<RefreshToken>
            {
                Filter = t => t.UserId == userId && t.ExpiresAtUtc > DateTime.UtcNow,
                AsNoTracking = false,
                OrderBy = q => q.OrderBy(t => t.CreatedAtUtc)
            };

            var tokens = await _unitOfWork.Repository<RefreshToken>().GetAsync(query, cancellationToken);
            foreach (var token in tokens)
            {
                await RevokeRefreshTokenAsync(token, ipAddress, reason, cancellationToken);
            }

            _logger.LogSection("OLD TOKENS REVOKED", $"🧹 All old tokens revoked for user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogSection("OLD TOKEN ERROR", $"❌ Error revoking old tokens: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken existingToken, string createdByIp, CancellationToken cancellationToken = default)
    {
        _logger.LogSection("ROTATE TOKEN", $"♻️ Rotating refresh token for user {existingToken.User?.Email}");

        try
        {
            await RevokeRefreshTokenAsync(existingToken, createdByIp, "TokenRotated", cancellationToken);

            if (existingToken.User is null)
            {
                _logger.LogSection("Not Found User", $"Not Found User Cant Generate Refresh Token is User Not Found {existingToken.User?.Email}");
                throw new NotFoundException("Not Found User Cant Generate Refresh Token is User Not Found");
            }

            var newToken = await CreateRefreshTokenAsync(existingToken.User, createdByIp, cancellationToken);

            existingToken.ReplacedByToken = newToken.Token;
            await _unitOfWork.Repository<RefreshToken>().Update(existingToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogSection("ROTATE SUCCESS", $"✅ Token rotated successfully for {existingToken.User?.Email}");
            return newToken;
        }
        catch (Exception ex)
        {
            _logger.LogSection("ROTATE ERROR", $"❌ Failed to rotate refresh token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var queryOption = new QueryOptions<RefreshToken>
            {
                Filter = rt => rt.Token.ToString().Trim() == refreshToken.ToString().Trim(),
                Includes = [rt => rt.User],
                AsNoTracking = false
            };

            var tokenEntity = await _unitOfWork.Repository<RefreshToken>()
                .GetSingleAsync(queryOption, cancellationToken);

            return tokenEntity is { IsActive: true } ? tokenEntity : null;
        }
        catch (Exception ex)
        {
            _logger.LogSection("VALIDATION ERROR", $"❌ Failed to validate refresh token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryOptions<RefreshToken>
            {
                Filter = t => t.UserId == userId && t.ExpiresAtUtc > DateTime.UtcNow,
                OrderBy = q => q.OrderByDescending(t => t.CreatedAtUtc)
            };

            var tokens = await _unitOfWork.Repository<RefreshToken>().GetAsync(query, cancellationToken);

            return tokens.First(); 
        }
        catch (Exception ex)
        {
            _logger.LogSection("FETCH ERROR", $"❌ Failed to get valid refresh token: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    private static string GenerateSecureToken(int length = 64)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
