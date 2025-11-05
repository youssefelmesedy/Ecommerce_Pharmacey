using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.Command.Login;
public class LoginHandler : IRequestHandler<LoginCommand, ResultDto<AuthenticationDto>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public LoginHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<AuthenticationDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(request.Dto.Email) || string.IsNullOrEmpty(request.Dto.Email))
            return _resultFactory.Failure<AuthenticationDto>("Email or Password cannot be empty","Cant Use Coulmen Empty");

        var result = await _authenticationService.LoginAsync(request.Dto, request.IpAddress!, cancellationToken);
        if(result is null || !result.IsActive)
            return _resultFactory.Failure<AuthenticationDto>(result!.Message, "Invalid email or password");

        return _resultFactory.Success(result, "Login successful");

    }
}
