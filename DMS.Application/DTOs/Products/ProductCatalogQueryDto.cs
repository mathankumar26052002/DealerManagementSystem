using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Products
{
    public class ProductCatalogQueryDto
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
