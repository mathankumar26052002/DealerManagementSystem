using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DMS.Application.DTOs.Orders
{
    public class RejectOrderRequestDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
    }
}
