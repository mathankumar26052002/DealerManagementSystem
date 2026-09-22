using DMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Infrastructure.Data.Configurations
{
    public class DealerConfiguration
     : IEntityTypeConfiguration<Dealer>
    {
        public void Configure(
            EntityTypeBuilder<Dealer> builder)
        {
            builder.ToTable("Dealers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DealerCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ContactPerson)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.DealerCode)
                .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
