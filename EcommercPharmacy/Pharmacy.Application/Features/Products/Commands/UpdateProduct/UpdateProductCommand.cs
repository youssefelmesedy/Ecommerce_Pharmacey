using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Products;

namespace Pharmacy.Application.Features.Products.Commands.UpdateProduct;
public record UpdateProductCommand : IRequest<ResultDto<Guid>>
{
    public UpdateProductDto Dto{ get; set; }

    public UpdateProductCommand(UpdateProductDto dto) => Dto = dto;
}
