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

        builder.Property(p => p.PhoneNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(pn => pn.IsPrimary)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(pn => pn.IsVerified)
               .IsRequired()
               .HasDefaultValue(false);

        // ✅ Index on phoneNumber (للبحث عن المستخدم برقم الهاتف)
        builder.HasIndex(p => p.PhoneNumber)
               .HasDatabaseName("IX_PhoneNumbers_Number");
    }
}
