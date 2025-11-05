using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
public interface IUserTokenService
{
    Task<UserToken> GetUserTokenByToken(string token, CancellationToken cancellationToken);
    Task<bool> CreateUserTokenAsync(UserToken userToken, CancellationToken cancellationToken);
    Task<bool> UpdateUserTokenAsync(UserToken userToken, CancellationToken cancellationToken);
    Task<bool> DeleteUserTokenAsync(Guid userId, CancellationToken cancellationToken);
}
