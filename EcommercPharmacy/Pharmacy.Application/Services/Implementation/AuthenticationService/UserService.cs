using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStreamService _file;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IFileStreamService file, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _file = file ?? throw new ArgumentNullException(nameof(file));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region 🔍 GetUserByEmailAsync
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        try
        {
            _logger.LogSection("USER LOOKUP", $"Searching user by email: {email}");

            var queryOption = new QueryOptions<User>
            {
                Filter = u => u.Email == email,
                AsNoTracking = true
            };

            var user = await _unitOfWork.Repository<User>().GetSingleAsync(queryOption, cancellationToken);

            if (user == null)
            {
                _logger.LogSection("USER NOT FOUND", $"⚠️ User with email '{email}' not found.", LogLevel.Warning);
                return null;
            }

            _logger.LogSection("USER FOUND", $"✅ User '{user.FullName}' fetched successfully.");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogSection("EXCEPTION", $"❌ Error fetching user by email '{email}': {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region ➕ CreateUserAsync
    public async Task<User> CreateUserAsync(User user, IFormFile? file, CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = new ValidateException();

            if (user is null)
                validation.AddError(nameof(user), "User object cannot be null.");

            if (string.IsNullOrWhiteSpace(user!.Email))
                validation.AddError(nameof(user.Email), "Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(user.FullName))
                validation.AddError(nameof(user.FullName), "Full name is required.");

            if (await _unitOfWork.Repository<User>().AnyAsync(u => u.Email == user.Email, cancellationToken))
                validation.AddError(nameof(user.Email), $"A user with email '{user.Email}' already exists.");

            if (await _unitOfWork.Repository<User>().AnyAsync(u => u.FullName == user.FullName, cancellationToken))
                validation.AddError(nameof(user.FullName), $"A user with full name '{user.FullName}' already exists.");

            if (validation.HasErrors)
                throw validation;

            _logger.LogSection("USER CREATION", $"Starting creation of user '{user.FullName}'");

            return await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                user.ProfileImageUrl = await SaveUserProfileImageAsync(user.Id, user.FullName, file, ct);

                await _unitOfWork.Repository<User>().AddAsync(user, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                _logger.LogSection("USER CREATED", $"✅ User '{user.Email}' created successfully (ID: {user.Id}).");
                return user;

            }, cancellationToken);
        }
        catch (ValidateException ex)
        {
            _logger.LogSection("VALIDATION FAILED", $"⚠️ Validation errors: {ex}", LogLevel.Warning);
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogSection("ARGUMENT ERROR", $"⚠️ {ex.Message}", LogLevel.Warning);
            throw new ValidateException(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogSection("EXCEPTION", $"❌ Unexpected error creating user '{user?.Email}': {ex.Message}", LogLevel.Error);
            throw new Exception("An unexpected error occurred while creating the user. Please try again later.", ex);
        }
    }
    #endregion

    #region ✏️ UpdateUserAsync
    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = new ValidateException();
            var repo = _unitOfWork.Repository<User>();
            var existingUser = await repo.GetByIdAsync(user.Id, cancellationToken);

            if (existingUser == null)
                throw new NotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(user.Email))
                validation.AddError(nameof(user.Email), "Email cannot be empty.");

            if (await repo.AnyAsync(u => u.Email == user.Email && u.Id != user.Id, cancellationToken))
                validation.AddError(nameof(user.Email), "Email already in use by another user.");

            if (validation.HasErrors)
                throw validation;

            await repo.Update(existingUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogSection("USER UPDATED", $"✅ User '{existingUser.Email}' updated successfully.");
            return existingUser;
        }
        catch (ValidateException ex)
        {
            _logger.LogSection("VALIDATION FAILED", $"⚠️ {ex}", LogLevel.Warning);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogSection("EXCEPTION", $"❌ Error updating user '{user.Id}': {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    #region ❌ DeleteUserAsync
    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.Repository<User>();
            var user = await repo.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                _logger.LogSection("DELETE SKIPPED", $"⚠️ User ID {userId} not found.", LogLevel.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                await _file.DeleteFileAsync(user.ProfileImageUrl, cancellationToken);

            await repo.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogSection("USER DELETED", $"✅ User '{user.Email}' deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogSection("EXCEPTION", $"❌ Error deleting user {userId}: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
    #endregion

    private async Task<string?> SaveUserProfileImageAsync(Guid userId, string fullName, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return null;

        return await _file.SaveFileAsync(userId, fullName, file, "Users", cancellationToken);
    }
}
