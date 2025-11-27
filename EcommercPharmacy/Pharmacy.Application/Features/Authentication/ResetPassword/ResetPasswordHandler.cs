using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.ResetPassword;
public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResultDto<string>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public ResetPasswordHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetPassword = await _authenticationService.ResetPasswordAsync(request.Dto, request.IpAddrese ?? "1952", cancellationToken);

        if (!resetPassword)
          return  _resultFactory.Failure<string>("Failed to process Reset password request.",
                "Cant send Email with null and Url with null");

        return _resultFactory.Success($"Reset Password Result = {resetPassword}", "Reset password email sent successfully.");
    }
}
