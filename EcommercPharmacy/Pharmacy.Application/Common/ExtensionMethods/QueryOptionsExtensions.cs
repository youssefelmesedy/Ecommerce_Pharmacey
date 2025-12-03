using Pharmacy.Infarstructure.Rpositoryies;

namespace Pharmacy.Application.Common.ExtensionMethods;
public static class QueryOptionsExtensions
{
    public static QueryOptions<TEntity> AddFilterParameter<TEntity>(this QueryOptions<TEntity> options, string key, object value)
    {
        if (options.FilterParameters == null)
        {
            options.FilterParameters = new Dictionary<string, object>();
        }
        options.FilterParameters[key] = value;

        return options;
    }
}
