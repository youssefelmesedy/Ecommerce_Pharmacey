using FluentValidation;
using Pharmacy.Application.Features.Categories.Commands.UpdateCategory;

namespace Pharmacy.Application.Features.Categories.validators;
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Dto.Id)
            .NotEmpty().WithMessage("Category Id is required.");

        RuleFor(x => x.Dto.Name)
            .MaximumLength(100).WithMessage("Category Name must not exceed 100 characters.");

        RuleFor(x => x.Dto.Description)
            .MaximumLength(500).WithMessage("Category Description must not exceed 500 characters.");
    }
}
