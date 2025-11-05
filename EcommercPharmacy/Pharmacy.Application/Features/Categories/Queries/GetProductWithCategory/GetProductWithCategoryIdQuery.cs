using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Queries.GetProductWithCategory;
public record GetProductWithCategoryIdQuery : IRequest<ResultDto<CategoryIncludeProductDto>>
{
    public Guid Category_Id { get; set; }

    public GetProductWithCategoryIdQuery(Guid category_Id) => Category_Id = category_Id;
}
