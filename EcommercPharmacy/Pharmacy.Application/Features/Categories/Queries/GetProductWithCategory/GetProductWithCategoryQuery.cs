using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Queries.GetProductWithCategory;
public record GetProductWithCategoryQuery: IRequest<ResultDto<IEnumerable<CategoryIncludeProductDto>>>
{
}
