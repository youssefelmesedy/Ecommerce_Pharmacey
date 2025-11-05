using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Categories.Queries.GetProductWithCategory;
public class GetProductWithCategoryHandler : IRequestHandler<GetProductWithCategoryQuery, ResultDto<IEnumerable<CategoryIncludeProductDto>>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IResultFactory _resultFactory;

    public GetProductWithCategoryHandler(ICategoryService categoryService, IMapper mapper, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<IEnumerable<CategoryIncludeProductDto>>> Handle(GetProductWithCategoryQuery request, CancellationToken cancellationToken)
    {
        var categoriesWithProducts = 
            _categoryService.GetCategoryIncludeProducts(cancellationToken);

        if (categoriesWithProducts == null)
            return _resultFactory.Failure<IEnumerable<CategoryIncludeProductDto>>(Messagies.NotFound,
                $"Not Found Categoies With Include Products");

        return _resultFactory.Success(await categoriesWithProducts, Messagies.Success);
    }
}