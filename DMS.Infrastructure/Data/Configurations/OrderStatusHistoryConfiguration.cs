using DMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Infrastructure.Data.Configurations
{
    public class OrderStatusHistoryConfiguration
      : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(
            EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ToTable("OrderStatusHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PreviousStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.NewStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ChangedAt)
                .IsRequired();

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.StatusHistories)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ChangedByUser)
                .WithMany(x => x.StatusHistories)
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
