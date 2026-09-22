namespace DMS.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductCodeSnapshot { get; set; } = string.Empty;

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }

        // Navigation properties

        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
