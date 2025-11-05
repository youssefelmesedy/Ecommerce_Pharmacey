using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Services.Implementation.AuthenticationService;

public class FileStreamService : IFileStreamService
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<FileStreamService> _logger;

    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long _maxFileSize = 2 * 1024 * 1024; // 2MB

    public FileStreamService(IHostEnvironment webHostEnvironment, ILogger<FileStreamService> logger)
    {
        _env = webHostEnvironment;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(Guid userId, string personFullName, IFormFile file, string folderName, CancellationToken cancellationToken = default)
    {
        try
        {
            var shortId = userId.ToString().Substring(0, 8);

            if (string.IsNullOrEmpty(shortId))
                throw new InvalidOperationException("User ID cannot be empty.");

            if (string.IsNullOrEmpty(personFullName))
                throw new InvalidOperationException("User name cannot be empty.");

            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file uploaded.");

            if (file.Length > _maxFileSize)
                throw new InvalidOperationException("File size cannot exceed 2 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only .jpg, .jpeg, and .png are allowed.");

            // 📂 المسار: wwwroot/images/{folderName}/{PersonFullName}_{shortId}
            var userFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "images", folderName, $"{personFullName}_{shortId}");
            if (!Directory.Exists(userFolder))
                Directory.CreateDirectory(userFolder);

            // 🗑️ احذف أي صورة قديمة
            foreach (var oldFile in Directory.GetFiles(userFolder))
            {
                File.Delete(oldFile);
            }

            // 📌 اسم جديد للصورة
            var fileName = $"{personFullName}_{shortId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(userFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            _logger.LogInformation("✅ File saved successfully at: {FilePath}", filePath);

            // ✅ URL نسبي يخزن في قاعدة البيانات
            return $"/images/{folderName}/{personFullName}_{shortId}/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error while saving file for user {UserName}", personFullName);
            throw new IOException("An error occurred while saving the file.", ex);
        }
    }

    public async Task<string> ReplaceFileAsync(Guid userId, string personFullName, IFormFile newFile, string oldFilePath, string folderName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (newFile == null || newFile.Length == 0)
                throw new InvalidOperationException("New file cannot be empty.");

            // 🧹 احذف الصورة القديمة لو كانت موجودة
            if (!string.IsNullOrWhiteSpace(oldFilePath))
            {
                await DeleteFileAsync(oldFilePath, cancellationToken);
            }

            // 💾 احفظ الصورة الجديدة
            var newPath = await SaveFileAsync(userId, personFullName, newFile, folderName, cancellationToken);

            _logger.LogInformation("✅ File replaced successfully for {UserName}", personFullName);

            return newPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error while replacing file for user {UserName}", personFullName);
            throw new IOException("An error occurred while replacing the file.", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            // 🔍 تحويل المسار النسبي إلى مسار فعلي
            var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("⚠️ File not found at path: {Path}", fullPath);
                return false;
            }

            await Task.Run(() => File.Delete(fullPath), cancellationToken);

            _logger.LogInformation("🗑️ File deleted successfully: {Path}", fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting file at path: {Path}", relativePath);
            throw new IOException("An error occurred while deleting the file.", ex);
        }
    }
}


