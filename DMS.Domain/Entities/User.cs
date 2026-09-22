namespace DMS.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int? DealerId { get; set; }

        // Navigation properties
        public Dealer? Dealer { get; set; }

        public ICollection<OrderStatusHistory> StatusHistories { get; set; }
            = new List<OrderStatusHistory>();
    }
}
