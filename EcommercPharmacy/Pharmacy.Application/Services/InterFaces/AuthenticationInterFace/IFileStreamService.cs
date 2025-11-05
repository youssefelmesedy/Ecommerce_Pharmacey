using Microsoft.AspNetCore.Http;

namespace Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
public interface IFileStreamService
{
    Task<string> SaveFileAsync(Guid userId, string PersonFullName, IFormFile file, string folderName, CancellationToken cancellationToken = default);
    Task<string> ReplaceFileAsync(Guid userId, string PersonFullName, IFormFile newFilePath, string oldFilePath, string folderName, CancellationToken cancellation);
    Task<bool> DeleteFileAsync(string relativePeth, CancellationToken cancellation);
}
