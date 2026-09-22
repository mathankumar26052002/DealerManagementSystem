using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class OrderStatusHistoryResponseDto
    {
        public int Id { get; set; }

        public string? PreviousStatus { get; set; }

        public string NewStatus { get; set; } = string.Empty;

        public int ChangedByUserId { get; set; }

        public string ChangedByUsername { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }

        public string? Remarks { get; set; }
    }
}
