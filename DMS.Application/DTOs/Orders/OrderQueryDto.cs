using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class OrderQueryDto
    {
        public string? Search { get; set; }

        public string? Status { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
