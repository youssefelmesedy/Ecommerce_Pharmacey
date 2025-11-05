using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Queries.Get;
public record GetCategoryQuery : IRequest<ResultDto<IEnumerable<CategoryDto>>>
{
}
