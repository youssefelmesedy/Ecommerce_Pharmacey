using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Common.Mappings.MappingCategories;
public partial class CategoriesProfile
{
    public void QueryCategories()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Id,
            opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name))

            .ForMember(dest => dest.Description,
            opt => opt.MapFrom(src => src.Description));
    }
}
