using FluentValidation;
using Pharmacy.Application.Features.Categories.Commands.DeleteCategory;

namespace Pharmacy.Application.Features.Categories.validators;
public class DeleteCategoryValidatior : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidatior()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category Id must not be empty.")
            .NotEqual(Guid.Empty).WithMessage("Category Id must be a valid GUID.");
    }
}
