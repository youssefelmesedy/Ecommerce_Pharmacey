using MediatR;
using Pharmacy.Application.Common.Models;

namespace Pharmacy.Application.Features.Products.Commands.DeleteProduct;
public record DeleteProductCommand : IRequest<ResultDto<bool>>
{
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id) => Id = id;
}
