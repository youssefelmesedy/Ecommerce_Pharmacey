using Microsoft.Extensions.Logging;
using Pharmacy.Application.Common.ExtenionFile;
using Pharmacy.Application.Common.Helpar;
using Pharmacy.Application.Common.Models;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;

namespace Pharmacy.Application.Services.Implementation.EntityService;
public class ProductService : GenericService<Product>, IProductService
{
    public ProductService(IUnitOfWork unitOfWork, ICacheService cache, ILogger<GenericService<Product>> logger)
        : base(unitOfWork, cache, logger)
    {
    }

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

            //Expression<Func<Product, bool>>? filter = null;

            //if (categoryId != null && categoryId != Guid.Empty)
            //    filter = p => p.CategoryId == categoryId;

            //if (!string.IsNullOrWhiteSpace(search))
            //{
            //    search = search.ToLower();
            //    filter = filter == null
            //        ? p => p.Name.ToLower().Contains(search)
            //        : filter.AndAlso(p => p.Name.ToLower().Contains(search));
            //}
            var filter = ExpressionExtensions.BuildProductFilter(
                name: search,
                categoryId: categoryId,
                minPrice: null,
                maxPrice: null);

            int totalCount = await _unitOfWork.Repository<Product>()
                .CountAsync(filter, cancellation);

            var query = new QueryOptions<Product>
            {
                Filter = filter,
                Skip = (pageNumber - 1) * pageSize,
                Take = pageSize,
                OrderBy = q => q.OrderBy(p => p.Name),
                Includes =
            {
                p => p.Category!,
                p => p.Images,
            }
            };

            var products = await _unitOfWork.Repository<Product>()
                .GetAsync(query, cancellation);

            return new PaginatedResult<Product>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogSection("Pagination Error",$"{ex}, Error in GetPagedProductAsync", LogLevel.Error);
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


    // --------------------------------------
    // 2️⃣ Override AddAsync (Add Business Logic)
    // --------------------------------------
    public override async Task<int> AddAsync(Product entity, CancellationToken cancellation = default)
    {
        await ValidateProductAsync(entity, isUpdate: false, cancellation);

        return  await base.AddAsync(entity, cancellation);
    }


    // --------------------------------------
    // 3️⃣ Override UpdateAsync (Add Business Logic)
    // --------------------------------------
    public override async Task<int> UpdateAsync(Product entity, CancellationToken cancellation = default)
    {
        await ValidateProductAsync(entity, isUpdate: true, cancellation);

        return await base.UpdateAsync(entity, cancellation);
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

}

