using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.EmailVerification;
public record EmailVerificationcommand : IRequest<ResultDto<string>>
{
    public EmailVerificationDto Dto { get; set; }

    public EmailVerificationcommand(EmailVerificationDto dto)
    {
        Dto = dto;
    }
}
