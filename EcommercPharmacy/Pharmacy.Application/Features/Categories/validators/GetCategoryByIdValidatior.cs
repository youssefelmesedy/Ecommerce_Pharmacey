using FluentValidation;
using Pharmacy.Application.Features.Categories.Queries.GetById;

namespace Pharmacy.Application.Features.Categories.validators;
public class GetCategoryByIdValidatior : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdValidatior()
    {
        RuleFor(x => x.Category_Id)
            .NotEmpty().WithMessage("Category Id must not be empty.")
            .NotEqual(Guid.Empty).WithMessage("Category Id must be a valid GUID.");
    }
}
