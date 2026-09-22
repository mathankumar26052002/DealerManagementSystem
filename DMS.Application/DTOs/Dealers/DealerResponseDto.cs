using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Dealers
{
    public class DealerResponseDto
    {
        public int Id { get; set; }
        public string DealerCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
