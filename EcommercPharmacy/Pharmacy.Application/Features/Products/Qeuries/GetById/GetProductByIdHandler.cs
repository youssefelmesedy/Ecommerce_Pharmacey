using AutoMapper;
using MediatR;
using Microsoft.Identity.Client;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Productes;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Products.Qeuries.GetById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ResultDto<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly IResultFactory _resultFactory;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IProductService productService, IResultFactory resultFactory, IMapper mapper)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResultDto<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        if(request.Id == Guid.Empty)
            return _resultFactory.Failure<ProductDto>(Messagies.InvalidProduct_ID, "Can't Return roduct With Id Null");

        var product = await _productService.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return _resultFactory.Failure<ProductDto>($"{Messagies.NotFound} Product", $"Can't Find Product With Id {request.Id}");

        var productDto = _mapper.Map<ProductDto>(product);

        return _resultFactory.Success(productDto, $"Product {Messagies.GetById}.");
    }
}
