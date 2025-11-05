using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Categories.Queries.GetProductWithCategory;
public class GetProductWithCategoryIdHandler : IRequestHandler<GetProductWithCategoryIdQuery, ResultDto<CategoryIncludeProductDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IResultFactory _resultFactory;

    public GetProductWithCategoryIdHandler(ICategoryService categoryService, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService),"The Category Service Null");
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory), "The Result Factory Null");
    }

    public async Task<ResultDto<CategoryIncludeProductDto>> Handle(GetProductWithCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var categryIncludeProduct = await _categoryService.GetCategoryIncludeProducts(request.Category_Id, cancellationToken);

        if (categryIncludeProduct is null)
            return _resultFactory.Failure<CategoryIncludeProductDto>(Messagies.GetByIdFailed,
                $"Not Found Category With Include Product With Id: {request.Category_Id}");

        return _resultFactory.Success(categryIncludeProduct, Messagies.Success);
    }
}
