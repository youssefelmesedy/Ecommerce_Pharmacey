using Pharmacy.Application.Dtos.Products;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Features.Products.Mapping
{
    public partial class ProductProfile
    {
        public void CurdMappingroduct()
        {
            // Create Mapper for CreateProductDto to Product and vice versa
            CreateMap<Product, CreateProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ReverseMap();

            // Update Mapper for UpdateProductDto to Product
            CreateMap<Product, UpdateProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ReverseMap();
        }
    }
}
