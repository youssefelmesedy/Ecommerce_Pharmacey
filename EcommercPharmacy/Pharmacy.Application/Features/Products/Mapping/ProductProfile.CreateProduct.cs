using Pharmacy.Application.Dtos.Productes;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Products.Mapping
{
    public partial class ProductProfile
    {
        public void MappingCreateProduct()
        {
            CreateMap<Product, CreateProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ReverseMap();
        }
    }
}
