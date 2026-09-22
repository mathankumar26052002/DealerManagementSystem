using DMS.Domain.Enum;

namespace DMS.Domain.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public OrderStatus PreviousStatus { get; set; }

        public OrderStatus NewStatus { get; set; }

        public int ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? Remarks { get; set; }

        // Navigation properties

        public Order Order { get; set; } = null!;

        public User ChangedByUser { get; set; } = null!;
    }
}
