using FluentValidation;
using Pharmacy.Application.Features.Products.Qeuries.GetById;

namespace Pharmacy.Application.Features.Products.validators;

public class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}