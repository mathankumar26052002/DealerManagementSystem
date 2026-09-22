using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Orders
{

    public class OrderResponseDto
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int DealerId { get; set; }

        public string DealerCode { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = [];

        public List<OrderStatusHistoryResponseDto> StatusHistory { get; set; } = [];
    }
}
