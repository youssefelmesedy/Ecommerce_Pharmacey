using Microsoft.Extensions.Logging;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using System.Linq.Expressions;

namespace Pharmacy.Application.Services.Implementation.EntityService;
public class CategoryService : GenericService<Category>, ICategoryService
{

    public CategoryService(IUnitOfWork unitOfWork, ICacheService cache, ILogger<GenericService<Category>> logger) 
        : base(unitOfWork, cache, logger)
    {
    }

    public async Task<IEnumerable<CategoryIncludeProductDto>> GetCategoryIncludeProducts(CancellationToken cancellation)
    {
        try
        {
            var queryOptions = new QueryOptions<Category>
            {
                OrderBy = c => c.OrderBy(c => c.Name),
                Includes = new List<Expression<Func<Category, object>>>
                {
                  c => c.products!
                }
            };

            var key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "GetCategoryIncludeProducts", queryOptions);

            // ✅ تأكد من انتظار الكاش
            var cachedData = await _cache.GetAsync<IEnumerable<CategoryIncludeProductDto>>(key, cancellation);
            if (cachedData is not null)
            {
                _logger.LogInformation("📦 Retrieved categories from cache (Key: {Key})", key);
                return cachedData;
            }

            var categories = await _unitOfWork.Repository<Category>()
                                              .GetAsync(queryOptions, cancellation);

            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("⚠️ No categories found for cache key: {Key}", key);
                return new List<CategoryIncludeProductDto>();
            }

            var result = categories
                .Select(c => new CategoryIncludeProductDto
                {
                    Category_Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? string.Empty,
                    Products = c.products != null
                        ? c.products.OrderBy(p => p.Name)
                            .Select(p => new ProductWithCategoryDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description ?? string.Empty,
                                Price = p.Price,
                                StockQuantity = p.StockQuantity,
                                CreatedAt = p.CreatedAt,
                                Images = p.Images ?? new List<ProductImage>()
                            }).ToList()
                        : new List<ProductWithCategoryDto>()
                })
                .ToList();

            // ✅ انتظر عملية الكاش
            await _cache.SetAsync(key, result, _cachePrefix,TimeSpan.FromMinutes(5), cancellation);

            _logger.LogInformation("✅ Loaded {Count} categories with products (Key: {Key})", result.Count, key);
            return result;
        }
        catch(Exception ex)
        {
            _logger.LogSection("LOGOUT ERROR", $"❌ Error during logout: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task<CategoryIncludeProductDto?> GetCategoryIncludeProducts(Guid category_Id, CancellationToken cancellation)
    {
        try
        {
            var key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "GetCategoryIncludeProductById", category_Id.ToString());

            return await _cache.GetOrSetAsync(key, async () =>
            {
                // 1️⃣ إعداد خيارات الكويري مع Include للـ Products
                var queryOptions = new QueryOptions<Category>
                {
                    Filter = c => c.Id == category_Id,
                    Includes = new List<Expression<Func<Category, object>>>
                    {
                        c => c.products!
                    },
                    AsNoTracking = true
                };

                // 2️⃣ جلب البيانات من قاعدة البيانات
                var category = await _unitOfWork.Repository<Category>()
                                                .GetSingleAsync(queryOptions, cancellation);

                //if (category is null)
                //     throw new ArgumentOutOfRangeException(nameof(category),$"The Id: {category_Id} Out Of Range Or Not Found");

                var result = new CategoryIncludeProductDto
                {
                    Category_Id = category!.Id,
                    Name = category.Name,
                    Description = category.Description ?? string.Empty,
                    Products = category.products != null
                        ? category.products
                            .OrderBy(p => p.Name)
                            .Select(p => new ProductWithCategoryDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description ?? string.Empty,
                                Price = p.Price,
                                StockQuantity = p.StockQuantity,
                                CreatedAt = p.CreatedAt,
                                Images = p.Images ?? new List<ProductImage>()
                            })
                            .ToList()
                        : new List<ProductWithCategoryDto>()
                };

                return result;

            }, _cachePrefix, TimeSpan.FromMinutes(5), cancellation);
        }
        catch (Exception ex)
        {
            _logger.LogSection("LOGOUT ERROR", $"❌ Error during logout: {ex.Message}", LogLevel.Error);
            throw;
        }
    }
}
