using FluentValidation;
using Pharmacy.Application.Features.Authentication.ResetPassword;

namespace Pharmacy.Application.Features.Authentication.Validation.ResetPassword;
public class ResetPasswordValidation : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidation()
    {
        RuleFor(x => x.Dto.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.Dto.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");

        RuleFor(x => x.Dto.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Dto.NewPassword).WithMessage("Confirm password must match the new password.");
    }
}
