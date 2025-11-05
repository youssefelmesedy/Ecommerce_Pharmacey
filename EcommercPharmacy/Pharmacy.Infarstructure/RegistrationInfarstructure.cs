using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Cacheing.ImplementationRedis;
using Pharmacy.Infarstructure.Persistens;
using Pharmacy.Infarstructure.UnitOfWorks.Implementation;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using Pharmacy.Infrastructure.Caching.ImplementationMemoryCache;
using Pharmacy.Infrastructure.Repositories.Implementations;
using Pharmacy.Infrastructure.Repositories.Interfaces;
using StackExchange.Redis;

namespace Pharmacy.Infarstructure
{
    public static class RegistrationInfarstructure
    {
        public static IServiceCollection RegistrationInfaructureServices
            (this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LocalServer");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'LocalServer' not found.");
            }

            Console.WriteLine($"🔗 Connection: {connectionString}");

            // Register other infrastructure services here (e.g., logging, caching, etc.)
            // ✅ Register DbContext with SQL Server provider
            services.AddDbContext<ApplicationDbContex>(options =>
                options.UseSqlServer(connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }
            )   );

            services.AddMemoryCache();

            // Register Memory Cache Service
            var cacheName = configuration["Cacheing:Name"];
            if (cacheName == "MemoryCache")
                services.AddSingleton<ICacheService, MemoryCacheService>();
            else
            {
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
                services.AddSingleton<ICacheService, RedisCacheService>();
            }

            RegisterApplicationServices(services);

            return services;
        }

        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            

            // Register a Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register a repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
