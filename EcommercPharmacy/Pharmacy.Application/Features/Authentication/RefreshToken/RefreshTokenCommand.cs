using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.RefreshToken;
public record RefreshTokenCommand : IRequest<ResultDto<RefreshTokenDto>>
{
    public string Token { get; init; }
    public string? IpAddress { get; init; }
    public RefreshTokenCommand(string token, string? ipAddress = ":1")
    {
        Token = token;
        IpAddress = ipAddress;
    }
}
