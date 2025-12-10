using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Products;

namespace Pharmacy.Application.Features.Products.Qeuries.Get;
public record GetAllProductQuery : IRequest<ResultDto<IEnumerable<ProductDto>>>
{
    public string? SearchByName { get; set; }

    public GetAllProductQuery(string? searchByName) => SearchByName = searchByName; 
}
