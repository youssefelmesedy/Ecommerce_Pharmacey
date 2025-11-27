using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.ForgotPassword;
public record ForgotPasswordCommand : IRequest<ResultDto<string>>
{
    public ForgotPasswordDto Dto { get; set; }

    public ForgotPasswordCommand(ForgotPasswordDto dto) => Dto = dto;
}
