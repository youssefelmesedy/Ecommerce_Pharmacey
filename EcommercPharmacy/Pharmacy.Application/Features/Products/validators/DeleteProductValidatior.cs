using FluentValidation;
using Pharmacy.Application.Features.Products.Commands.DeleteProduct;

namespace Pharmacy.Application.Features.Products.validators;
public class DeleteProductValidatior : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidatior()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product Id must not be empty.")
            .NotEqual(Guid.Empty).WithMessage("Product Id must be a valid GUID.");
    }
}
