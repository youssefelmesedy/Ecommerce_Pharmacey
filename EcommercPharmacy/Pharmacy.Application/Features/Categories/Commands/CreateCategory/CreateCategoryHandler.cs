using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Categories.Commands.CreateCategory;
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, ResultDto<CategoryDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IResultFactory _resultFactory;
    public CreateCategoryHandler(ICategoryService categoryService, IMapper mapper, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto is null)
            return  _resultFactory.Failure<CategoryDto>("Can't Insert Data Of Null");

        if (await _categoryService.AnyAsync(c => c.Name == request.Dto.Name))
            return _resultFactory.Failure<CategoryDto>(Messagies.Conflicted, Messagies.DuplicateCategoryName);

        var newCategory = _mapper.Map<Category>(request.Dto);

        await _categoryService.AddAsync(newCategory, cancellationToken);

        return _resultFactory.Success(_mapper.Map<CategoryDto>(newCategory), Messagies.Created);
    }
}
