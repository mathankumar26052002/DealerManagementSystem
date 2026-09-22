using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Dashboard
{
    public class AdminDashboardResponseDto
    {
        public int TotalDealers { get; set; }

        public int ActiveDealers { get; set; }

        public int TotalProducts { get; set; }

        public int ActiveProducts { get; set; }

        public int SubmittedOrders { get; set; }

        public int ApprovedOrders { get; set; }
    }
}
