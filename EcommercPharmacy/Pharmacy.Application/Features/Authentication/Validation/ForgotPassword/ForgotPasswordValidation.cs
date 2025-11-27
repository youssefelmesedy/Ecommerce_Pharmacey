using FluentValidation;
using Pharmacy.Application.Features.Authentication.ForgotPassword;

namespace Pharmacy.Application.Features.Authentication.Validation.ForgotPassword;
public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidation()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Dto.BaseUrl)
                   .NotEmpty().WithMessage("BaseURL is required.")
                   .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                   .WithMessage("Invalid BaseURL format.");
    }
}
