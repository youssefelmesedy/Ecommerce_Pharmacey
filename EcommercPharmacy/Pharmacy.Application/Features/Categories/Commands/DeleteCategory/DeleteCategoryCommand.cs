using MediatR;
using Pharmacy.Application.Common.Models;

namespace Pharmacy.Application.Features.Categories.Commands.DeleteCategory;
public class DeleteCategoryCommand : IRequest<ResultDto<bool>>
{
    public Guid CategoryId { get; set; }

    public DeleteCategoryCommand(Guid categoryId) => CategoryId = categoryId;
}
