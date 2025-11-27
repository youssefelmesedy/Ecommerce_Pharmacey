using AutoMapper;

namespace Pharmacy.Application.Features.Products.Mapping;
public partial class ProductProfile : Profile
{
    public ProductProfile()
    {
        MappingPage();
        MappingCreateProduct();
    }
}
