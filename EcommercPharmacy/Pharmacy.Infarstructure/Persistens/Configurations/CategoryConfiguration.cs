using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Description).HasMaxLength(500);

        builder.HasIndex(c => c.Name)
               .HasDatabaseName("Ix_Categories_Name");

        builder.HasMany(c => c.products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
