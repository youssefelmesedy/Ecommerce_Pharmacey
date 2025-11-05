using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Commands.CreateCategory;
public record CreateCategoryCommand : IRequest<ResultDto<CategoryDto>>
{
    public CreateCategoryDto Dto { get; set; }

    public CreateCategoryCommand(CreateCategoryDto dto) => Dto = dto;
}
