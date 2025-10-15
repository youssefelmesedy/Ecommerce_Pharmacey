using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.Infarstructure.Persistens;

namespace Pharmacy.Infarstructure
{
    public static class Registration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
                }));
            return services;
        }
    }
}
