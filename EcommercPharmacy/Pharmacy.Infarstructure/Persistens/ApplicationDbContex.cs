using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens;
public class ApplicationDbContex : DbContext
{
    public ApplicationDbContex(DbContextOptions<ApplicationDbContex> options) : base(options)
    {
    }

    public DbSet<User> users => Set<User>();
    public DbSet<PhoneNumbers> phoneNumbers => Set<PhoneNumbers>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> productImages => Set<ProductImage>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItems> OrderItems => Set<OrderItems>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContex).Assembly);
    }
}
