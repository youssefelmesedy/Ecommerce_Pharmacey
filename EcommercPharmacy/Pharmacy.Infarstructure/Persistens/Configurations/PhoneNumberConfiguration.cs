using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infarstructure.Persistens.Configurations;

internal class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumbers>
{
    public void Configure(EntityTypeBuilder<PhoneNumbers> builder)
    {
        builder.ToTable("PhoneNumbers");

        builder.HasKey(pn => pn.Id);

        builder.Property(pn => pn.PhoneNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(pn => pn.IsPrimary)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(pn => pn.IsVerified)
               .IsRequired()
               .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(pn => pn.PhoneNumber)
               .HasDatabaseName("IX_PhoneNumbers_PhoneNumber");

        builder.HasIndex(pn => pn.UserId)
               .HasDatabaseName("IX_PhoneNumbers_UserId");

        // Relationship
        builder.HasOne(pn => pn.User)
               .WithMany(u => u.PhoneNumbers)
               .HasForeignKey(pn => pn.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
