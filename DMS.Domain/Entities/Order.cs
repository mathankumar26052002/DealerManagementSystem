using DMS.Domain.Enum;

namespace DMS.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int DealerId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SubmittedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Navigation properties

        public Dealer Dealer { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<OrderStatusHistory> StatusHistories { get; set; }
            = new List<OrderStatusHistory>();
    }
}
