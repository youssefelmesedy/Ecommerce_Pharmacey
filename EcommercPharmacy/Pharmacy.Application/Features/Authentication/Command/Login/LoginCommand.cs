using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.Command.Login;
public record LoginCommand : IRequest<ResultDto<AuthenticationDto>>
{
    public LoginDto Dto { get; set; }
    public string? IpAddress { get; set; }

    public LoginCommand(LoginDto dto, string? ipAddress = ":1")
    {
        Dto = dto;
        IpAddress = ipAddress;
    }
}
