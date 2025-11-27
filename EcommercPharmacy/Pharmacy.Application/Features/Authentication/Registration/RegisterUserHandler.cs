using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.Registration;
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, ResultDto<AuthenticationDto>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public RegisterUserHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<AuthenticationDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RegisterAsync(request.Dto, request.IpAddress!, cancellationToken);
        if(!result.IsActive)
            return _resultFactory.Failure<AuthenticationDto>("Registration failed.");

        return _resultFactory.Success(result, "Registration successful.");
    }
}
