using FluentValidation;
using Pharmacy.Application.Features.Categories.Commands.CreateCategory;

namespace Pharmacy.Application.Features.Categories.validators;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Dto.Description)
            .MaximumLength(500)
            .WithMessage("Category description must not exceed 500 characters.");
    }
}
