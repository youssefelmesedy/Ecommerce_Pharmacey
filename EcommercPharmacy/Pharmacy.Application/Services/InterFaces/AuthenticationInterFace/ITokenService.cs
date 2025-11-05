using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
public interface ITokenService
{
    /// <summary>Create new RefreshToken for given user and persist it.</summary>
    Task<RefreshToken> CreateRefreshTokenAsync(User user, string createdByIp, CancellationToken cancellationToken = default);

    /// <summary>Rotate existing refresh token -> revoke old, create new, persist both.</summary>
    Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken existingToken, string createdByIp, CancellationToken cancellationToken = default);

    /// <summary>Revoke a refresh token (mark revoked and set RevokedByIp).</summary>
    Task RevokeRefreshTokenAsync(RefreshToken token, string revokedByIp, string reason = null!, CancellationToken cancellationToken = default);
    Task RevokeOldTokensAsync(Guid userId, string IpAddress, string reason = null!, CancellationToken cancellationToken = default);

    /// <summary>Validate a refresh token string for given user id and return RefreshToken entity if valid.</summary>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetValidRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}
