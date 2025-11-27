using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Productes;

namespace Pharmacy.Application.Features.Products.Commands.CreateProduct;
public record CreateProductCommand : IRequest<ResultDto<Guid>>
{
    public CreateProductDto Dto { get; set; }

    public CreateProductCommand(CreateProductDto dto) => Dto = dto;
}
