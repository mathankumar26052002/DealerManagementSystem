using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class OrderItemResponseDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }
    }
}
