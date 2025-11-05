using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.Command.Logout;
public class LogoutHanddler : IRequestHandler<LogoutCommand, ResultDto<string>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public LogoutHanddler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
         var logout = await _authenticationService.LogoutAsync(request.Token, request.IpAddress!, cancellationToken);
        if (string.IsNullOrWhiteSpace(logout))
            return _resultFactory.Failure<string>("Lougout Failed", "Someting Withs Rong");

        return _resultFactory.Success(logout, "logout");
        
    }
}
