using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Categories.Queries.GetById;
public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, ResultDto<CategoryDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IResultFactory _resultFactory;

    public GetCategoryByIdHandler(ICategoryService categoryService, IMapper mapper, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Category_Id == Guid.Empty)
            return _resultFactory.Failure<CategoryDto>(Messagies.GetByIdFailed, 
                $"Can't Use Empty Id {request.Category_Id}");

        var category = await _categoryService.GetByIdAsync(request.Category_Id, cancellationToken);

        if (category is null)
            return _resultFactory.Failure<CategoryDto>(
                Messagies.GetByIdFailed,
                $"No category found with ID: {request.Category_Id}");

        var categoryDto = _mapper.Map<CategoryDto>(category);

        return _resultFactory.Success(categoryDto, Messagies.Success);
    }
}
