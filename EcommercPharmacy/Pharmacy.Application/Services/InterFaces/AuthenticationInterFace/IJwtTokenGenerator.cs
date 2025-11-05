using Pharmacy.Domain.Entities;
using System.Security.Claims;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
public interface IJwtTokenGenerator
{
    /// <summary>Generates JWT (access token) with claims from user and returns token string and expiry.</summary>
    Task<(string token, DateTime expiresAtUtc)> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<string> GeneratingShortTermTokens(User user, string action);

    /// <summary>Validate token and return ClaimsPrincipal even if token expired (set validateLifetime false) for refresh flow.</summary>
    ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime = true);
}
