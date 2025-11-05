using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Rpositoryies;

namespace Pharmacy.Application.Features.Categories.Queries.Get;
public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, ResultDto<IEnumerable<CategoryDto>>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IResultFactory _resultFactory;

    public GetCategoryHandler(ICategoryService categoryService, IMapper mapper, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<IEnumerable<CategoryDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Category>
        {
            AsNoTracking = true,
            OrderBy = q => q.OrderBy(c => c.Name)
        };
        
        var categories = await _categoryService.GetAsync(options, cancellationToken);

        if (!categories.Any())
            return _resultFactory.Failure<IEnumerable<CategoryDto>>(Messagies.GetAllFailed,
                        "Can't Get Data But Can No Data In Data Base Or Can't Insert Data");

        var category = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        return _resultFactory.Success(category, Messagies.Success);
    }
}
