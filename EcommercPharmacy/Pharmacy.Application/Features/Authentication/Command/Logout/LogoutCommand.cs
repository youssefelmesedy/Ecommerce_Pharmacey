using MediatR;
using Pharmacy.Application.Common.Models;

namespace Pharmacy.Application.Features.Authentication.Command.Logout;
public record LogoutCommand : IRequest<ResultDto<String>>
{
    public string Token { get; init; }
    public string? IpAddress { get; init; }
    public LogoutCommand(string token, string? ipAddress = ":1")
    {
        Token = token;
        IpAddress = ipAddress;
    }
}
