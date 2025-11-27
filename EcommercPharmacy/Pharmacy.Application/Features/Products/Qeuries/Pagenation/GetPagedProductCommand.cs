using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Productes;

namespace Pharmacy.Application.Features.Products.Qeuries.Pagenation;
public record GetPagedProductCommand : IRequest<ResultDto<PaginatedResult<ProductDto>>>
{
    public GetPagedProductCommand(int pageNumber, int pageSize, Guid? categoryId, string? search)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        CategoryId = categoryId;
        Search = search;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Search { get; set; }
}
