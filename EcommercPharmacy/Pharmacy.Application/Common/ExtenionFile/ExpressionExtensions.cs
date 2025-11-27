using Pharmacy.Domain.Entities;
using System.Linq.Expressions;

namespace Pharmacy.Application.Common.ExtenionFile
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(

                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<Product, bool>> BuildProductFilter(
          string? name,
          Guid? categoryId,
          decimal? minPrice,
          decimal? maxPrice)
        {
            Expression<Func<Product, bool>> predicate = p => true;

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.AndAlso(p => p.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                predicate = predicate.AndAlso(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                predicate = predicate.AndAlso(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                predicate = predicate.AndAlso(p => p.Price <= maxPrice.Value);
            }

            return predicate;
        }

    }
}
