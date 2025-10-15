using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pharmacy.Infarstructure.Persistens;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContex>
{
    public ApplicationDbContex CreateDbContext(string[] args)
    {
        // 🧠 Get current path (for EF CLI)
        var basePath = Directory.GetCurrentDirectory();

        // 🔍 Look for the appsettings.json from API project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        // ⚙️ Get connection string
        var connectionString = configuration.GetConnectionString("LocalServer");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContex>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContex(optionsBuilder.Options);
    }
}
