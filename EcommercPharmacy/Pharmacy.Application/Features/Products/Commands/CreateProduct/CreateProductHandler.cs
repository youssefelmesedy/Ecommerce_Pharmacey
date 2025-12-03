using AutoMapper;
using MediatR;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Common.StaticMessages;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ResultDto<Guid>>
    {
        private readonly IProductService _productService;
        private readonly IResultFactory _resultFactory;
        private readonly IMapper _mapper;
        public CreateProductHandler(IProductService productService, IResultFactory resultFactory, IMapper mapper)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResultDto<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request.Dto);

           var result = await _productService.AddAsync(product,cancellationToken);

            if (result <= 0)
            {
                return _resultFactory.Failure<Guid>($"{Messagies.CreateFailed} Product");
            }

            return _resultFactory.Success(product.Id, $"Product {Messagies.Created}.");
        }
    }
}
