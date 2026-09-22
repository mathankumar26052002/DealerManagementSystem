using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class UpdateOrderItemRequestDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
