using Pharmacy.Application.Dtos.Products;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Products.Mapping;

public partial class ProductProfile
{
    public void MappingPage()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.Images,
                opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)));
    }
}
