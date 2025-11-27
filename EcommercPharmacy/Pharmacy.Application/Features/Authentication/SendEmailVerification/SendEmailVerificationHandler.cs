using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.SendEmailVerification;

public class SendEmailVerificationHandler : IRequestHandler<SendEmailVerificationCommand, ResultDto<string>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public SendEmailVerificationHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<string>> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var (Message, Success) = await _authenticationService.SendEmailVerificationAsync(request.Dto);

        if(!Success)
            return _resultFactory.Failure<string>("Failed to send email verification.", Message);

        return _resultFactory.Success<string>(Success.ToString(), Message);
    }
}
