using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.Dtos.Products;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;

namespace Pharmacy.Application.Features.Products.Qeuries.Pagenation;

public class GetPagedProductHandler
    : IRequestHandler<GetPagedProductQuery, ResultDto<PaginatedResult<ProductDto>>>
{
    private readonly IProductService _productService;
    private readonly IResultFactory _resultFactory;
    private readonly IMapper _mapper;
    public GetPagedProductHandler(IProductService productService, IResultFactory resultFactory, IMapper mapper)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResultDto<PaginatedResult<ProductDto>>> Handle(GetPagedProductQuery request, CancellationToken cancellationToken)
    {
        var paginated = await _productService.GetPagedProductAsync(
            request.PageNumber,
            request.PageSize,
            request.CategoryId,
            request.Search
        );

        if (paginated.TotalCount == 0 || paginated.Data is null)
            return _resultFactory.Failure<PaginatedResult<ProductDto>>(
                $"{Messagies.NotFound}",
                "No products match the given criteria."
            );

        var paginatedResult = new PaginatedResult<ProductDto>
        {
            Data = _mapper.Map<IEnumerable<ProductDto>>(paginated.Data),
            TotalCount = paginated.TotalCount,
            PageNumber = paginated.PageNumber,
            PageSize = paginated.PageSize
        };

        return _resultFactory.Success(paginatedResult,
            $"{Messagies.GetPagination} products."
        );
    }
}
