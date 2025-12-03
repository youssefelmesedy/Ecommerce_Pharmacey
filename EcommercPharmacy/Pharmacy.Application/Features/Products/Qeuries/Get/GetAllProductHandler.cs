using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.ExtenionFile;
using Pharmacy.Application.Common.ExtensionMethods;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Productes;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Rpositoryies;

namespace Pharmacy.Application.Features.Products.Qeuries.Get;

public class GetAllProductHandler : IRequestHandler<GetAllProductQuery, ResultDto<IEnumerable<ProductDto>>>
{
    private readonly IProductService _productService;
    private readonly IResultFactory _resultFactory;
    private readonly IMapper _mapper;

    public GetAllProductHandler(IProductService productService, IResultFactory resultFactory, IMapper mapper)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }

    public async Task<ResultDto<IEnumerable<ProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        var filter = ExpressionExtensions.BuildProductFilter(request.SearchByName, null, null, null);

        var query = new QueryOptions<Product>
        {
            FilterExpression = filter,
            AsNoTracking = false,
            Includes =
            {
                i => i.Images!,
                c => c.Category!
            },
            OrderBy = p => p.OrderBy(n => n.Name),
        }.AddFilterParameter("Name",request.SearchByName ?? "Not_Filter");

        var products = await _productService.GetAsync(query, cancellationToken);

        if (!products.Any())
            return _resultFactory.Failure<IEnumerable<ProductDto>>($"{Messagies.NotFound}", $"Not Insert Data In data base The Count Product: {products.Count()}");

        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        if (productsDto is null)
            throw new BusinessException(Messagies.MappingFailed);

        return _resultFactory.Success(productsDto, $"{Messagies.GetAll} Product ");
    }
}
