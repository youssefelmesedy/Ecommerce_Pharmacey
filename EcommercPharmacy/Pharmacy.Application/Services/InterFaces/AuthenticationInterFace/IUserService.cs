using Microsoft.AspNetCore.Http;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
// تعريف واجهة لخدمات المستخدم
public interface IUserService
{
    // استرجاع مستخدم بناءً على البريد الإلكتروني (أو يمكن أن تكون حسب معرف)
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    // إنشاء مستخدم جديد
    Task<User> CreateUserAsync(User user, IFormFile file, CancellationToken cancellationToken = default);

    // تحديث بيانات المستخدم
    Task<User> UpdateUserAsync(User updatedUser, CancellationToken cancellationToken = default);

    // حذف مستخدم حسب معرف
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
