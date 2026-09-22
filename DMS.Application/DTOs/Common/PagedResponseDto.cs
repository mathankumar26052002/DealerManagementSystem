using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Common
{
    public class PagedResponseDto<T>
    {
        public List<T> Items { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
