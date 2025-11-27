using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.ResetPassword;
public record ResetPasswordCommand : IRequest<ResultDto<string>>
{
    public ResetPasswordDto Dto { get; set; }
    public string? IpAddrese { get; set; }

    public ResetPasswordCommand(ResetPasswordDto dto, string? ipAddrese = null)
    {
        Dto = dto;
        IpAddrese = ipAddrese;
    }
}
