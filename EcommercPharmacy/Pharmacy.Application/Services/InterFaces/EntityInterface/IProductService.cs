using Pharmacy.Application.Common.Models;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Services.InterFaces;

namespace Pharmacy.Application.Services.InterFaces.EntityInterface;
public interface IProductService : IGenericService<Product>
{
    Task<PaginatedResult<Product>> GetPagedProductAsync(
        int pageNumber,
        int pageSize,
        Guid? categoryId = null,
        string? search = null,
        CancellationToken cancellation = default);

    Task<int> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
