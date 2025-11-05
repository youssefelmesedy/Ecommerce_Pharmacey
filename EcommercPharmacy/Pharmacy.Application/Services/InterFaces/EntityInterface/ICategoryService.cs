using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Services.InterFaces;

namespace Pharmacy.Application.Services.InterFaces.EntityInterface;
public interface ICategoryService : IGenericService<Category>
{
    Task<IEnumerable<CategoryIncludeProductDto>> GetCategoryIncludeProducts(CancellationToken cancellation);
    Task<CategoryIncludeProductDto?> GetCategoryIncludeProducts(Guid category_Id, CancellationToken cancellation);
}
