using FluentValidation;
using Pharmacy.Application.Features.Authentication.Registration;

namespace Pharmacy.Application.Features.Authentication.Validation.Registration;
public class RegisterValidation : AbstractValidator<RegisterUserCommand>
{
    public RegisterValidation()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Dto.FullName)
            .NotEmpty().WithMessage("Full Name is required.");

        RuleFor(x => x.Dto.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Dto.phoneNumbers)
            .NotEmpty().WithMessage("At least one phone number is required.");

        RuleFor(x => x.Dto.ImageProfile)
            .Must(file => file == null || file.Length > 0).WithMessage("Invalid profile image file.");
    }
}
