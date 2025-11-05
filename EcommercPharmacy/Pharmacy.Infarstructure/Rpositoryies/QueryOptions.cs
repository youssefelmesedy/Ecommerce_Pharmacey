using System.Linq.Expressions;

namespace Pharmacy.Infarstructure.Rpositoryies
{
    public record QueryOptions<TEntity>
    {
        public Expression<Func<TEntity, bool>>? Filter { get; set; }
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; }
        public Expression<Func<TEntity, object>>? GroupBy { get; set; }
        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new();
        public bool AsNoTracking { get; set; } = true;
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }

}
