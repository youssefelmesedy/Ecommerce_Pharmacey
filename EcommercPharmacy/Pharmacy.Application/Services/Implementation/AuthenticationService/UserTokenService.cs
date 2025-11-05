using Microsoft.Extensions.Logging;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using System.Linq.Expressions;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService;
public class UserTokenService : GenericService<UserTokenService>, IUserTokenService
{
    public UserTokenService(IUnitOfWork unitOfWork, ICacheService cache, ILogger<GenericService<UserTokenService>> logger)
        : base(unitOfWork, cache, logger)
    {
    }
    
    public async Task<UserToken> GetUserTokenByToken(string token, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return null!;

            var queryOptions = new QueryOptions<UserToken>
            {
                Filter = ut => ut.Token == token,
                AsNoTracking = true,
                Includes = new List<Expression<Func<UserToken, object>>>
                {
                    ut => ut.User!
                }
            };

            var userToken = await _unitOfWork.Repository<UserToken>()
                .GetSingleAsync(queryOptions, cancellationToken);

            if (userToken is null)
                return null!;

            return userToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting user token.");
            throw;
        }
    }

    public async Task<bool> CreateUserTokenAsync(UserToken userToken, CancellationToken cancellationToken)
    {
        try
        {
            if(userToken is null)
                throw new ArgumentNullException(nameof(userToken));

            await _unitOfWork.Repository<UserToken>().AddAsync(userToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating user token.");
            throw;
        }
    }

    public async Task<bool> UpdateUserTokenAsync(UserToken userToken, CancellationToken cancellationToken)
    {
        if (userToken is null)
            throw new ArgumentNullException(nameof(userToken));

        await _unitOfWork.Repository<UserToken>().Update(userToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteUserTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        if(userId == Guid.Empty)
            throw new ArgumentNullException(nameof(userId));

        var userToken = await _unitOfWork.Repository<UserToken>()
            .GetSingleAsync(new QueryOptions<UserToken>
            {
                Filter = ut => ut.UserId == userId
            }, cancellationToken);
        if(userToken == null)
            return false;

        await _unitOfWork.Repository<UserToken>().Delete(userToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
