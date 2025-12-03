using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Products.Commands.UpdateProduct;

public class UpdatedProductHandler : IRequestHandler<UpdateProductCommand, ResultDto<Guid>>
{
    private readonly IProductService _productService;
    private readonly IResultFactory _resultFactory;
    private readonly IMapper _mapper;

    public UpdatedProductHandler(IProductService productService, IResultFactory resultFactory, IMapper mapper)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResultDto<Guid>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productEntity = _mapper.Map<Product>(request.Dto);

        if (productEntity is null)
            return _resultFactory.Failure<Guid>($"{Messagies.MappingFailed} Product");

        var updatedProduct = await _productService.UpdateAsync(productEntity, cancellationToken);

        if (updatedProduct <=  0)
            return _resultFactory.Failure<Guid>($"{Messagies.UpdateFailed} Product", $"updatedProduct Effected Row: {updatedProduct}");

        return _resultFactory.Success(productEntity.Id, $"Product {Messagies.Updated}");
    }
}
