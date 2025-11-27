using MediatR;
using Microsoft.Win32;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.ForgotPassword;
public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ResultDto<string>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public ForgotPasswordHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var forgotPassword = await _authenticationService.ForgotPasswordAsync(request.Dto, cancellationToken);
        if(forgotPassword)
            _resultFactory.Failure<string>("Failed to process forgot password request.", 
                "Cant send Email with null and Url with null");

        return _resultFactory.Success($"Forgot Password Result = {forgotPassword}", "Forgot password email sent successfully.");
    }
}
