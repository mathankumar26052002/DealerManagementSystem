using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class CreateOrderRequestDto
    {
        public List<CreateOrderItemRequestDto> Items { get; set; } = [];
    }
}
