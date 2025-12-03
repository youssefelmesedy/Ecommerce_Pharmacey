using AutoMapper;
using Microsoft.Extensions.Logging;
using Pharmacy.Application.Common.ExtenionFile;
using Pharmacy.Application.Common.ExtensionMethods;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Dtos.Productes;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;

namespace Pharmacy.Application.Services.Implementation.EntityService;
public class ProductService : GenericService<Product>, IProductService
{
    private readonly IMapper _mapper;
    public ProductService(IUnitOfWork unitOfWork, ICacheService cache, ILogger<GenericService<Product>> logger, IMapper mapper)
        : base(unitOfWork, cache, logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException($"Can't Instainse Create Mapping  {nameof(mapper)}");
    }
    // --------------------------------------
    public async Task<PaginatedResult<Product>> GetPagedProductAsync(
    int pageNumber,
    int pageSize,
    Guid? categoryId = null,
    string? search = null,
    CancellationToken cancellation = default)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var filter = ExpressionExtensions.BuildProductFilter(
                name: search,
                categoryId: categoryId,
                minPrice: null,
                maxPrice: null);

            int totalCount = await _unitOfWork.Repository<Product>()
                .CountAsync(filter, cancellation);

            var query = new QueryOptions<Product>
            {
                FilterExpression = filter,
                Skip = (pageNumber - 1) * pageSize,
                Take = pageSize,
                OrderBy = q => q.OrderBy(p => p.Name),
                Includes =
                {
                    p => p.Category!,
                    p => p.Images,
                },
                AsNoTracking = false,
            }
            .AddFilterParameter("Name", search ?? "Not_Filter")
            .AddFilterParameter("CategoryId", categoryId ?? default);

            var cacheKey = CacheKeyBuilder.BuilderCacheKey(
                _cachePrefix, "Paged", pageNumber, pageSize, query);

            // -------- FIX 1: Type-safe cache reading ----------
            var cachedData = await _cache.GetAsync<PaginatedResult<Product>>(cacheKey, cancellation);
            if (cachedData is not null)
                return cachedData;

            // -------- Fetch from DB ----------
            var products = await _unitOfWork.Repository<Product>()
                .GetAsync(query, cancellation);

            // -------- Save to Cache ----------
            var result = new PaginatedResult<Product>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            await _cache.SetAsync(
                cacheKey,
                result,
                _cachePrefix,
                TimeSpan.FromMinutes(5),
                cancellation);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogSection("Pagination Error", $"{ex}, Error in GetPagedProductAsync", LogLevel.Error);
            throw;
        }
    }

    // --------------------------------------
    // 1️⃣ Validate Product (Common business rules)
    // --------------------------------------
    private async Task ValidateProductAsync(Product product, bool isUpdate, CancellationToken cancellation)
    {
        // Rule: Price > 0
        if (product.Price <= 0)
            throw new BusinessException("Price must be greater than zero.");

        // Rule: Stock >= 0
        if (product.StockQuantity < 0)
            throw new BusinessException("Stock cannot be negative.");

        // Rule: Category must exist
        bool categoryExists = await _unitOfWork.Repository<Category>()
            .AnyAsync(c => c.Id == product.CategoryId, cancellation);

        if (!categoryExists)
            throw new BusinessException("Category does not exist.");

        // Rule: Unique name inside same category
        bool nameExists = await _unitOfWork.Repository<Product>()
            .AnyAsync(p =>
                p.Name == product.Name &&
                p.CategoryId == product.CategoryId &&
                (!isUpdate || p.Id != product.Id),
                cancellation);

        if (nameExists)
            throw new BusinessException("Product name already exists in this category.");

        // Rule: Only 1 main image allowed
        if (product.Images.Count(i => i.IsMain) > 1)
            throw new BusinessException("Only one image can be marked as main.");
    }

    // Get Product By Id Overreide
    public override async Task<Product?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        var query = new QueryOptions<Product>
        {
            FilterExpression = p => p.Id.Equals(id),
            Includes =
            {
                p => p.Images,
                p => p.Category!
            },

            AsNoTracking = false
        };
        string key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "ById", id);

        return await _cache.GetOrSetAsync(key, async () =>
        {
            try
            {
                return await _unitOfWork.Repository<Product>().FirstOrDefaultAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error fetching {typeof(Product).Name} by ID {id}");
                throw;
            }
        }, _cachePrefix, TimeSpan.FromMinutes(10), cancellationToken);
       
    }

    // --------------------------------------
    // 2️⃣ Override AddAsync (Add Business Logic)
    // --------------------------------------
    public override async Task<int> AddAsync(Product entity, CancellationToken cancellation = default)
    {
        await ValidateProductAsync(entity, isUpdate: false, cancellation);

        var effected =  await base.AddAsync(entity, cancellation);

        await _cache.RemoveByPrefixAsync("Category", cancellation);

        return effected;
    }


    // --------------------------------------
    // 3️⃣ Override UpdateAsync (Add Business Logic)
    // --------------------------------------
    public override async Task<int> UpdateAsync(Product entity, CancellationToken cancellation = default)
    {
        var existing = await _unitOfWork.Repository<Product>().GetByIdAsync(entity.Id, cancellation)
            ?? throw new BusinessException("Product not found.");

        // Apply Partial Update Product 
        ApplyPartialUpdate(existing, _mapper.Map<UpdateProductDto>(entity));

        // Validate Updated Product
        await ValidateProductAsync(existing, isUpdate: true, cancellation);

        var effected = await base.UpdateAsync(existing, cancellation);

        await _cache.RemoveByPrefixAsync("Category", cancellation);

        return effected;
    }

    // override DeleteAsync 
    public  async Task<int> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Product>().GetByIdAsync(id, cancellationToken);
            if (entity is null)
                return 0;

            var effected = await base.DeleteAsync(entity, cancellationToken);

            await _cache.RemoveByPrefixAsync("Category", cancellationToken);

            return effected;
        }
        catch (Exception ex)
        {
            _logger.LogSection("Delete Failed", $"Product Not Foun Or Occurding Data,\n {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    // --------------------------------------
    // 4️⃣ Decrease Stock (Special Business Logic)
    // --------------------------------------
    public async Task<int> DecreaseStockAsync(Guid productId, decimal amount, CancellationToken cancellation)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId, cancellation)
            ?? throw new BusinessException("Product not found.");

        if (product.StockQuantity < amount)
            throw new BusinessException("Not enough stock.");

        product.StockQuantity -= amount;

        await _unitOfWork.Repository<Product>().Update(product);

        var effected = await _unitOfWork.SaveChangesAsync(cancellation);

        await _cache.RemoveByPrefixAsync(_cachePrefix, cancellation);

        return effected;
    }


    // --------------------------------------
    // 5️⃣ Mark Product as Out-of-Stock
    // --------------------------------------
    public async Task<int> MarkOutOfStockAsync(Guid productId, CancellationToken cancellation)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId, cancellation)
            ?? throw new BusinessException("Product not found.");

        product.StockQuantity = 0;

        await _unitOfWork.Repository<Product>().Update(product);

        var effected = await _unitOfWork.SaveChangesAsync(cancellation);

        await _cache.RemoveByPrefixAsync(_cachePrefix, cancellation);

        return effected;
    }

    private void ApplyPartialUpdate(Product existing, UpdateProductDto dto)
    {
        // 🔹 Name
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != "string")
            existing.Name = dto.Name.Trim();

        // 🔹 Description
        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != "string")
            existing.Description = dto.Description.Trim();

        // 🔹 Price
        if (dto.Price.HasValue && dto.Price.Value > 0)
            existing.Price = dto.Price.Value;

        // 🔹 StockQuantity
        if (dto.StockQuantity.HasValue && dto.StockQuantity.Value >= 0)
            existing.StockQuantity = dto.StockQuantity.Value;

        // 🔹 CategoryId
        if (dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty)
            existing.CategoryId = dto.CategoryId.Value;

    }
}

