using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Application.Common.Mappings.MappingCategories;
public partial class CategoriesProfile
{
    public void CreateCategories()
    {
        CreateMap<Category, CreateCategoryDto>()
            .ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description,
            opt => opt.MapFrom(src => src.Description)).ReverseMap();
    }

    public void UpdateCategories()
    {
        CreateMap<Category, UpdateCategoryDto>()
            .ReverseMap()
            .ForMember(dest => dest.Name,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name))) // شرط بس لما تكون القيمة مش فاضية

            .ForMember(dest => dest.Description,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.Description)));
    }

}
