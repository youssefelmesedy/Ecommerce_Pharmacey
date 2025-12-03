using Pharmacy.Infarstructure.Rpositoryies;
using System.Reflection;

namespace Pharmacy.Infarstructure.Cacheing;

public static class CacheKeyBuilder
{
    public static string BuilderCacheKey(params object?[] parts)
    {
        return string.Join("_", parts.Select(p =>
        {
            if (p == null) return "null";

            var type = p.GetType();

            // ✅ لو النوع هو QueryOptions<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("QueryOptions"))
            {
                var method = typeof(CacheKeyBuilder)
                    .GetMethod(nameof(BuildQueryOptionsKey), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(type.GetGenericArguments()[0]);

                return (string)method.Invoke(null, new[] { p })!;
            }

            return p switch
            {
                DateTime dt => dt.ToString("yyyyMMddHHmmss"),
                DateTimeOffset dto => dto.ToString("yyyyMMddHHmmss"),
                Guid guid => guid.ToString(),
                Delegate del => del.Method.Name,
                Exception exp => exp.Message,
                _ => p.ToString() ?? "Prefix_Unknown"
            };
        }));
    }

    private static string BuildQueryOptionsKey<T>(QueryOptions<T> options)
    {
        // Includes
        var includes = options.Includes != null && options.Includes.Any()
            ? string.Join(",", options.Includes.Select(i => i.Body.ToString().Replace("c => c.", "")))
            : "none";

        // OrderBy
        var orderBy = options.OrderBy != null
            ? options.OrderBy.Method.Name
            : "none";

        // FilterParameters
        var filter = "none";
        if (options.FilterParameters != null && options.FilterParameters.Any())
            filter = string.Join("_", options.FilterParameters.Select(kv => $"{kv.Key}:{kv.Value}"));

        return $"Filter:{filter}" +
               $"_OrderBy:{orderBy}" +
               $"_Includes:{includes}" +
               $"_AsNoTracking:{options.AsNoTracking}" +
               $"_Skip:{options.Skip?.ToString() ?? "n"}" +
               $"_Take:{options.Take?.ToString() ?? "n"}";
    }

}
