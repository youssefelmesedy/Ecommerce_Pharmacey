using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Authentication;

namespace Pharmacy.Application.Features.Authentication.SendEmailVerification;
public record SendEmailVerificationCommand : IRequest<ResultDto<string>> 
{
    public SendEmailVerificationDto Dto { get; set; }

    public SendEmailVerificationCommand(SendEmailVerificationDto dto)
    {
        Dto = dto;
    }
}
