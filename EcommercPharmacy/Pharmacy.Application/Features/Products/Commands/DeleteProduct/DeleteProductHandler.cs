using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ResultDto<bool>>
{
    private readonly IProductService _productService;
    private readonly IResultFactory _resultFactory;
    public DeleteProductHandler(IProductService productService, IResultFactory resultFactory)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
    }

    public async Task<ResultDto<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if(request.Id.ToString() is null && request.Id == Guid.Empty)
            return _resultFactory.Failure<bool>(Messagies.InvalidProduct_ID,"Can't Delete Product With Id Null and with Guid Empty");

        var result = await _productService.DeleteProductAsync(request.Id, cancellationToken);
        if (result == 0)
            return _resultFactory.Failure<bool>($"Product {Messagies.DeleteFailed}", "Failed to delete the product. It may not exist.");

        return _resultFactory.Success(true, $"Product {Messagies.Deleted}");
    }
}