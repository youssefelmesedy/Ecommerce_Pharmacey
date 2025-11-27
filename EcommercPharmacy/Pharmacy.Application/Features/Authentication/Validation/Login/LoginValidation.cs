using FluentValidation;
using Pharmacy.Application.Features.Authentication.Login;

namespace Pharmacy.Application.Features.Authentication.Validation.Login;
public class LoginValidation : AbstractValidator<LoginCommand> 
{
    public LoginValidation()
    {
        RuleFor(l => l.Dto.Email)
           .NotEmpty().WithMessage("Can't send Email Empty");

        RuleFor(l => l.Dto.Password)
           .NotEmpty().WithMessage("Can't send Password Empty");
    }
}
