using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.EmailVerification;
public class EmailVerificationHandler  : IRequestHandler<EmailVerificationcommand, ResultDto<string>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public EmailVerificationHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<string>> Handle(EmailVerificationcommand request, CancellationToken cancellationToken)
    {
        var result= await _authenticationService.EmailVerificationAsync(request.Dto, cancellationToken);

        if (!result)
            return _resultFactory.Failure<string>("Some error with email verification.");

        return _resultFactory.Success(result.ToString(), "Email verification link sent successfully.");
    }
}
