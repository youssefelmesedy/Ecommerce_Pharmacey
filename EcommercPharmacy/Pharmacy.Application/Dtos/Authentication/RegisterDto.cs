using Microsoft.AspNetCore.Http;
using Pharmacy.Application.Common.ExtenionFile;
using Pharmacy.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Application.Dtos.Authentication;
public record RegisterDto
{
    [Required(ErrorMessage = "Full Name is Required")]
    [MaxLength(100, ErrorMessage = "Full Name must be at most 100 characters long.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    [MaxLength(15, ErrorMessage = "Password must be at most 100 characters long.")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; init; } = string.Empty;

    public List<string> phoneNumbers { get; init; } = new List<string>();
    public string? Address { get; init; }

    [MaxFileSize(5 * 1024 * 1024)] // 5 MB
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
    public IFormFile ImageProfile { get; init; } = null!;
}
