using FluentValidation;
using Pharmacy.Application.Features.Products.Commands.UpdateProduct;

namespace Pharmacy.Application.Features.Products.validators;
public class UpdateProductValidation : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidation()
    {
        RuleFor(p => p.Dto.Id)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(p => p.Dto.Name)
            .MaximumLength(100).WithMessage("Product Name must not exceed 100 characters.");
        RuleFor(p => p.Dto.Description)
            .MaximumLength(500).WithMessage("Product Description must not exceed 500 characters.");
        RuleFor(p => p.Dto.Price)
            .GreaterThan(0).WithMessage("Product Price must be greater than zero.");
        RuleFor(p => p.Dto.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock Quantity cannot be negative.");
        RuleFor(p => p.Dto.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}
