using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;

namespace Pharmacy.Application.Features.Authentication.RefreshToken;
public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ResultDto<RefreshTokenDto>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IResultFactory _resultFactory;

    public RefreshTokenHandler(IAuthenticationService authenticationService, IResultFactory resultFactory)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<RefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RefreshTokenAsync(request.Token, request.IpAddress!, cancellationToken);
        if (result is null)
            return _resultFactory.Failure<RefreshTokenDto>("Some These Error In Generate RefreshToken", 
                "Cant Return Access Token and Token With Null");

        return _resultFactory.Success(result, "Generated Refresh Token SuccessFully..");
    }
}
