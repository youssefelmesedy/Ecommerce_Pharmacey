using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Categories;

namespace Pharmacy.Application.Features.Categories.Commands.UpdateCategory;
public record UpdateCategoryCommand : IRequest<ResultDto<CategoryDto>>
{
    public UpdateCategoryDto Dto { get; set; }

    public UpdateCategoryCommand(UpdateCategoryDto dto) => Dto = dto;
}
