using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.Command.RegisterCommand;
public record RegisterUserCommand : IRequest<ResultDto<AuthenticationDto>>
{
    public RegisterDto Dto { get; set; }

    public string? IpAddress { get; set; }
    public RegisterUserCommand(RegisterDto dto, string? ipAdress = null)
    {
        Dto = dto;
        IpAddress = ipAdress;
    }    
}
