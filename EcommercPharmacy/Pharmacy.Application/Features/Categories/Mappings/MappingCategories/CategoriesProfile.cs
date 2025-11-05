using AutoMapper;

namespace Pharmacy.Application.Common.Mappings.MappingCategories;
public partial class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        QueryCategories();
        CreateCategories();
        UpdateCategories(); 
    }
}
