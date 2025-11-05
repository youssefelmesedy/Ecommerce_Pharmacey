using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Categories.Commands.DeleteCategory;
public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ResultDto<bool>>
{
    private readonly ICategoryService _categoryService;
    private readonly IResultFactory _resultFactory;

    public DeleteCategoryHandler(ICategoryService categoryService, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var existCategory = await _categoryService.GetByIdAsync(request.CategoryId);
        if (existCategory == null)
            return _resultFactory.Failure<bool>(Messagies.GetByIdFailed, 
                                  $"The Categry With Use Id: {request.CategoryId} Not Found..!");

        await _categoryService.DeleteAsync(existCategory, cancellationToken);

        return _resultFactory.Success(true, Messagies.Deleted);
    }
}
