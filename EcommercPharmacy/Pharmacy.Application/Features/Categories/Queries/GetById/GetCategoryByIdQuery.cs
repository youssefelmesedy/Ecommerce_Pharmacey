using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Queries.GetById;
public record GetCategoryByIdQuery : IRequest<ResultDto<CategoryDto>>
{
    public Guid Category_Id { get; set; }

    public GetCategoryByIdQuery(Guid category_Id) => Category_Id = category_Id;
}
