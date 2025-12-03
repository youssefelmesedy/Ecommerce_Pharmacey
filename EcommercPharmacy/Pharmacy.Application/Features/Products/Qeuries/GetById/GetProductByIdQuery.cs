using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Productes;

namespace Pharmacy.Application.Features.Products.Qeuries.GetById;
public record GetProductByIdQuery : IRequest<ResultDto<ProductDto>>
{
    public Guid Id { get; set; }
    public GetProductByIdQuery(Guid id) => Id = id;
}
