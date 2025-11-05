using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Categories.Commands.UpdateCategory;
public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ResultDto<CategoryDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IResultFactory _resultFactory;

    public UpdateCategoryHandler(ICategoryService categoryService, IMapper mapper, IResultFactory resultFactory)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _categoryService.GetByIdAsync(request.Dto.Id);
        if (entity is null)
            return _resultFactory.Failure<CategoryDto>
                (Messagies.GetByIdFailed,$"The Ctegory Id: {request.Dto.Id}, Was Not Found Use Category");

        if (!string.IsNullOrWhiteSpace(request.Dto.Name))
        {
            var normalizedName = request.Dto.Name.Trim().ToLower();

            bool exists = await _categoryService.AnyAsync(c =>
                c.Name.ToLower().Trim() == normalizedName && c.Id != request.Dto.Id, cancellationToken);

            if (exists)
                return _resultFactory.Failure<CategoryDto>(Messagies.Namealreadyexists,
                    $"The Category Was Already Use Name: {entity.Name}...!");
        }

        // Mapping: Only update fields that have values
        var updatedEntity = _mapper.Map(request.Dto, entity);

        await _categoryService.UpdateAsync(updatedEntity, cancellationToken);

        return _resultFactory.Success(
            _mapper.Map<CategoryDto>(updatedEntity),
            $"Category {Messagies.Updated}"
        );
    }
}
