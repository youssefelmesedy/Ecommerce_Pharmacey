using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Common.ExtenionFile;
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is not null)
        {
            var file = (Microsoft.AspNetCore.Http.IFormFile)value;
            if (file.Length > _maxFileSize)
            {
                return new ValidationResult($"Maximum allowed file size is { _maxFileSize / (1024 * 1024)} MB.");
            }
        }
        return ValidationResult.Success;
    }
}
