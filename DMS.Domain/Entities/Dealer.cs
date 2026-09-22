namespace DMS.Domain.Entities
{
    public class Dealer
    {
        public int Id { get; set; }

        public string DealerCode { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public ICollection<User> Users { get; set; }
            = new List<User>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}
